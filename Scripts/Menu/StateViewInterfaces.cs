using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IInitView
{
	void IInitView(StateView sv);
}

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