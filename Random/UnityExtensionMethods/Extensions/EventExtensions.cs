using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventInt : UnityEvent<int> { }

[Serializable]
public class UnityEventFloat : UnityEvent<float> { }

[Serializable]
public class UnityEventString : UnityEvent<string> { }

[Serializable]
public class UnityEventBool : UnityEvent<bool> { }

[Serializable]
public class UnityEventVector3 : UnityEvent<Vector3> { }

[Serializable]
public class UnityEventVector2 : UnityEvent<Vector2> { }

[Serializable]
public class UnityEventGameObject : UnityEvent<GameObject> { }

[Serializable]
public class UnityEventQuaternion : UnityEvent<Quaternion> { }

[Serializable]
public class UnityEventTransform : UnityEvent<Transform> { }

[Serializable]
public class UnityEventColor : UnityEvent<Color> { }

[Serializable]
public class UnityEventTexture2D : UnityEvent<Texture2D> { }

[Serializable]
public class UnityEventAudioClip : UnityEvent<AudioClip> { }