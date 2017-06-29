module.exports = function (grunt) {

    var secret = grunt.file.readJSON('secret.json');
    var sshOptions = {
        host: secret.testhost,
        username: secret.testusername,
        privateKey: grunt.file.read(secret.sshkeypath)
    }

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        sshexec: {
            restart: {
                command: [
                    "pwd",
                    "echo 'restarting service'",
                    "service alpaca restart"
                ],
                options: sshOptions
            },
            setup: {
                command: [
                    'apt-get update',
                    'apt-get upgrade -y',
                    'echo "Storage=persistent" >> /etc/systemd/journald.conf',
                    'echo "SystemMaxUse=100M" >> /etc/systemd/journald.conf',
                    'echo "ForwardToSyslog=no" >> /etc/systemd/journald.conf',
                    'sh -c \'echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ xenial main" > /etc/apt / sources.list.d / dotnetdev.list\'',
                    'apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 417A0893',
                    'apt-get update',
                    'apt-get install dotnet-sdk-2.0.0-preview2-006497 -y'
                ],
                options: sshOptions
            }
        }
    });

    grunt.loadNpmTasks('grunt-ssh');
};