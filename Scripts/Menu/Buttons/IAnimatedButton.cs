using System.Collections;

public interface IAnimatedButton
{
	void SetOwner(AnimatedButton owner);
	IEnumerator Idle();
	IEnumerator Press();
	IEnumerator Select();
	IEnumerator Release();
	IEnumerator Click();

}