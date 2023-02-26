using System.Collections;
using UnityEngine;

public static class AnimationHelper
{
	public static IEnumerator TriggerAndWaitForAnimation(Animator animator, string trigger)
	{
		animator.SetTrigger(trigger);
		//	Wait for transition to complete
		yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

		//	Wait for animation to complete
		yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
	}
}
