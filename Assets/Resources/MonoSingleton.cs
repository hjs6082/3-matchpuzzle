using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{ 
	public static T instance { get; private set; }
	void Awake() => instance = FindObjectOfType(typeof(T)) as T; 
}