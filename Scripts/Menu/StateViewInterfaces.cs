using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IExecuteView
{
	IEnumerator Execute();
}

public interface ISetupView
{
	Task SetupView();
}

public interface IOpenView
{
	IEnumerator Open();
}

public interface ICloseView
{
	IEnumerator Close();
}