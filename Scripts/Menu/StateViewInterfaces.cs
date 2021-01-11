using System.Collections;
using Cysharp.Threading.Tasks;

public interface IInitView
{
	void InitView(StateView sv);
}

public interface IExecuteView
{
	IEnumerator Execute();
}

public interface ISetupView
{
	UniTask SetupView();
}

public interface IOpenView
{
	IEnumerator Open();
}

public interface ICloseView
{
	IEnumerator Close();
}