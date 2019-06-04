using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExecuteView
{
	IEnumerator Execute();
}

public interface IOpenView
{
	IEnumerator Open();
}

public interface ICloseView
{
	IEnumerator Close();
}