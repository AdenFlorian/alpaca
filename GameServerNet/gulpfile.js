const exec = require('child_process').exec
const spawn = require('child_process').spawn;
const copydir = require('copy-dir')
const fs = require('fs')
const gulp = require('gulp')
const rimraf = require('rimraf')
const client = require('scp2')
const node_ssh = require('node-ssh')
const ssh2Client = require('ssh2').Client;

const secretFilePath = './secret.json'

const projectName = 'GameServerNet'
const projectFolder = `.`
const projectFileName = `${projectName}.csproj`
const projectFilePath = `${projectFolder}/${projectFileName}`

const projectBinFolder = `${projectFolder}/bin`
const buildOutputFolder = `${projectBinFolder}/debug/netcoreapp2.0`
const publishFolder = `${buildOutputFolder}/publish`

const tarFileName = `${projectName}.tar.gz`

const testFolder = 'test'

var secret

if (fs.existsSync(secretFilePath)) {
	secret = JSON.parse(fs.readFileSync(secretFilePath))
	// Add all the gruntfile tasks to gulp
	require('gulp-grunt')(gulp)
} else {
	gulp.stop("***Run 'node setup.js' before using gulp!***")
}

gulp.task('clean', (cb) => {
	cleanTarTask(() => {
		rimraf(projectBinFolder, (err) => {
			if (err) throw err
			rimraf(projectBinFolder, (err) => {
				if (err) throw err
				cb()
			})
		})
	})
})

gulp.task('restore', myShellTask(`dotnet restore ${projectFilePath}`))

gulp.task('dotnet-build', ['restore'], myShellTask(`dotnet build ${projectFilePath}`))

gulp.task('build', ['dotnet-build'], () => {
	copySync('config/dev/config.json', buildOutputFolder + '/config.json')
	copyTokenSync(secret.devbottoken, buildOutputFolder)
	//copywindowsbinsbuildSync()
})

gulp.task('run', ['build'], myShellTask(`dotnet ${projectName}.dll`, { cwd: buildOutputFolder }))

gulp.task('publish', ['restore'], (cb) => {
	myShell(`dotnet publish ${projectFilePath}`, null, () => {
		//copylinuxbinspublishSync()
		//copyTokenSync(secret.testbottoken, publishFolder)
		//console.log('Copying config to publish folder...')
		//copySync('config/test/config.json', publishFolder + '/config.json')
		cb()
	})
})

gulp.task('deploy', ['publish'], (cb) => {
	myShell('node do tar', null, () => {
		UploadToTestServerUsingSftpTask(() => {
			sshDeploy(() => {
				cleanTarTask(cb)
			})
		})
	})
})

gulp.task('ssh-deploy', sshDeploy)

function sshDeploy(cb) {
	var destinationFolder = "alpaca"
	const ssh = new node_ssh()

	ssh.connect({
		host: secret.testhost,
		username: secret.testusername,
		privateKey: secret.sshkeypath
	}).then(() => {
		doSshCommands(ssh, [
			"pwd",
			"echo 'stopping alpaca service'",
			"service alpaca stop",
			"echo 'deleting old app'",
			`rm -rf ${destinationFolder}`,
			`mkdir ${destinationFolder}`,
			"echo 'unpacking new app'",
			`tar xzf ${tarFileName} -C ${destinationFolder}`,
			`chmod +x ${destinationFolder}/${projectName}.dll`,
			"echo 'starting alpaca service'",
			"service alpaca start"
		], () => {
			ssh.dispose()
			cb()
		})
	})
}

function doSshCommands(ssh, commands, cb) {
	if (commands.length === 0) {
		cb()
	} else {
		ssh.execCommand(commands.shift()).then((result) => {
			if (result.stdout.length > 0) console.log('STDOUT: ' + result.stdout)
			if (result.stderr.length > 0) console.log('STDERR: ' + result.stderr)
			doSshCommands(ssh, commands, cb)
		})
	}
}

// ***Start test commands***

gulp.task('test', myShellTask('dotnet test test/BundtBotTests/BundtBotTests.csproj',
	{ verbose: true }))

// ***Start remote server commands***

gulp.task('rlogs', (cb) => {
	var conn = new ssh2Client();
	conn.on('ready', () => {
		console.log('Client :: ready');
		conn.exec('journalctl -f -o cat -u alpaca.service', (err, stream) => {
			if (err) throw err;
			stream.on('close', (code, signal) => {
				console.log('Stream :: close :: code: ' + code + ', signal: ' + signal);
				conn.end();
				cb()
			}).on('data', (data) => {
				console.log(data.toString().trim());
			}).stderr.on('data', (data) => {
				console.log(data.toString().trim());
			});
		});
	}).connect({
		host: secret.testhost,
		port: 22,
		username: secret.testusername,
		privateKey: fs.readFileSync(secret.sshkeypath)
	});
})

gulp.task('setup-server', myShellTask('grunt sshexec:setup'))

gulp.task('restart-remote', myShellTask('grunt sshexec:restart'))

// ***functions***

function copyTokenSync(token, outputFolder) {
	console.log('Copying token...')
	fs.writeFileSync(`${outputFolder}/bottoken`, token)
}

function copywindowsbinsbuildSync() {
	//console.log('Copying windows binaries to output folder...')
	//copySync('bin/opus/windows-1.1.2-x86-64/opus.dll', buildOutputFolder + '/libopus.dll')
}

function copylinuxbinspublishSync() {
	//console.log('Copying linux binaries to publish folder...')
	//copySync('bin/opus/linux-1.1.2-x86-64/libopus.so.0.5.2', publishFolder + '/libopus.dll')
}

function UploadToTestServerUsingSftpTask(cb) {
	console.log(`Uploading ${tarFileName} to ${secret.testhost}...`)

	client.defaults({
		port: 22,
		host: secret.testhost,
		username: secret.testusername,
		privateKey: fs.readFileSync(secret.sshkeypath)
	})

	client.on('transfer', (buffer, uploaded, total) => {
		if (uploaded % 25 == 0) {
			console.log(uploaded + '/' + total)
		}
	})

	client.upload(tarFileName, tarFileName, () => {
		client.close()
		cb()
	})
}

function cleanTarTask(cb) {
	fs.unlink(`${projectName}.tar.gz`, (err) => {
		if (err) console.log(err)
		if (cb) cb()
	})
}

function myShell(command, options, cb) {
	myShellTask(command, options)(cb)
}

function myShellTask(command, options) {
	return (cb) => {
		var split = command.split(' ')
		var cmd = split[0]
		var args = []

		for (var i = 1; i < split.length; i++) {
			args[i - 1] = split[i]
		}
		console.log(`Running command '${cmd}' with args '${args}'`)

		var commandSpawn

		if (options) {
			commandSpawn = spawn(cmd, args, options)
		} else {
			commandSpawn = spawn(cmd, args)
		}

		commandSpawn.stdout.on('data', (data) => {
			console.log(`${data.toString().trim()}`)
		})

		commandSpawn.stderr.on('data', (data) => {
			console.log(`stderr: ${data.toString().trim()}`)
		})

		commandSpawn.on('error', (err) => {
			console.error('on error: ' + err.toString().trim())
		})

		commandSpawn.on('close', (code) => {
			console.log(`child process exited with code ${code}`)
			cb()
		})
	}
}

function copySync(srcFile, destFile) {
	fs.createReadStream(srcFile).pipe(fs.createWriteStream(destFile))
}
