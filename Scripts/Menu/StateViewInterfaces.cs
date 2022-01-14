using System.Collections;
using System.Threading.Tasks;

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