using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSettings : MonoBehaviour
{
	public static LogSettings I;

	public bool Info;
	public bool Trace;
	public bool Error;
	public bool Warning;

	public string TraceFilter;

	void Awake()
	{
		I = this;
	}

	void Update()
	{
		
	}
}
