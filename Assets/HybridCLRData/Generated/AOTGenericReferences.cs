public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder`1<System.Object>
	//Cysharp.Threading.Tasks.UniTask`1<System.Object>
	//Cysharp.Threading.Tasks.UniTask`1/Awaiter<System.Object>
	//GameFramework.Fsm.FsmState`1<System.Object>
	//GameFramework.Fsm.IFsm`1<System.Object>
	//GameFramework.GameFrameworkAction`1<System.Object>
	//GameFramework.GameFrameworkAction`2<System.Byte,UnityEngine.Vector3>
	//GameFramework.ObjectPool.IObjectPool`1<System.Object>
	//Google.Protobuf.Collections.RepeatedField`1<System.Int64>
	//Google.Protobuf.Collections.RepeatedField`1<System.Int32>
	//Google.Protobuf.Collections.RepeatedField`1<System.Object>
	//Google.Protobuf.IDeepCloneable`1<System.Object>
	//Google.Protobuf.IMessage`1<System.Object>
	//Google.Protobuf.MessageParser`1<System.Object>
	//SerializableDictionary`2<System.Object,System.Object>
	//Singleton`1<System.Object>
	//SingletonMono`1<System.Object>
	//System.Action`1<System.Int64>
	//System.Action`1<System.Object>
	//System.Action`1<System.Int32>
	//System.Action`2<System.Object,System.Object>
	//System.Action`2<System.Byte,System.Object>
	//System.Collections.Generic.Dictionary`2<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2<cfg.Error.EErrorCode,System.Object>
	//System.Collections.Generic.Dictionary`2<TextShowComponentType,System.Object>
	//System.Collections.Generic.Dictionary`2<UIFormId,System.Int32>
	//System.Collections.Generic.Dictionary`2<System.Object,System.Int32>
	//System.Collections.Generic.Dictionary`2<System.Int32,System.Object>
	//System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>
	//System.Collections.Generic.Dictionary`2/Enumerator<TextShowComponentType,System.Object>
	//System.Collections.Generic.Dictionary`2/Enumerator<UIFormId,System.Int32>
	//System.Collections.Generic.Dictionary`2/Enumerator<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2/Enumerator<System.Object,System.Int32>
	//System.Collections.Generic.Dictionary`2/ValueCollection<System.Int32,System.Object>
	//System.Collections.Generic.Dictionary`2/ValueCollection/Enumerator<System.Int32,System.Object>
	//System.Collections.Generic.HashSet`1<System.Object>
	//System.Collections.Generic.HashSet`1<System.Int32>
	//System.Collections.Generic.HashSet`1/Enumerator<System.Object>
	//System.Collections.Generic.IEnumerator`1<System.Object>
	//System.Collections.Generic.IList`1<System.Object>
	//System.Collections.Generic.KeyValuePair`2<System.Object,System.Int32>
	//System.Collections.Generic.KeyValuePair`2<System.Int64,System.Object>
	//System.Collections.Generic.KeyValuePair`2<TextShowComponentType,System.Object>
	//System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>
	//System.Collections.Generic.KeyValuePair`2<System.Int32,System.Object>
	//System.Collections.Generic.KeyValuePair`2<UIFormId,System.Int32>
	//System.Collections.Generic.LinkedList`1<System.Object>
	//System.Collections.Generic.LinkedListNode`1<System.Object>
	//System.Collections.Generic.List`1<UnityEngine.Touch>
	//System.Collections.Generic.List`1<System.Object>
	//System.Collections.Generic.List`1<UnityEngine.EventSystems.RaycastResult>
	//System.Collections.Generic.List`1<System.Int16>
	//System.Collections.Generic.List`1<System.Double>
	//System.Collections.Generic.List`1<System.Int32>
	//System.Collections.Generic.List`1<System.Int64>
	//System.Collections.Generic.List`1<System.Single>
	//System.Collections.Generic.List`1<UnityEngine.Vector3>
	//System.Collections.Generic.List`1<UnityEngine.Vector2>
	//System.Collections.Generic.List`1/Enumerator<System.Object>
	//System.Collections.Generic.List`1/Enumerator<UnityEngine.Touch>
	//System.Collections.Generic.List`1/Enumerator<System.Int32>
	//System.Collections.Generic.Queue`1<System.Int64>
	//System.Collections.Generic.Queue`1<System.Int32>
	//System.Collections.Generic.Queue`1<System.Object>
	//System.Collections.Generic.SortedDictionary`2<System.Int64,System.Object>
	//System.Collections.Generic.SortedDictionary`2/Enumerator<System.Int64,System.Object>
	//System.Comparison`1<System.Object>
	//System.EventHandler`1<System.Object>
	//System.Func`1<System.Object>
	//System.Func`2<System.Object,Cysharp.Threading.Tasks.UniTask`1<System.Object>>
	//System.Func`3<System.Object,System.Int32,System.Object>
	//System.IEquatable`1<System.Object>
	//System.Nullable`1<UnityEngine.Color>
	//System.Nullable`1<System.Int32>
	//System.Predicate`1<UnityEngine.Touch>
	//System.Predicate`1<System.Object>
	//System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Byte>
	//System.Runtime.CompilerServices.TaskAwaiter`1<System.Byte>
	//System.Runtime.CompilerServices.TaskAwaiter`1<System.Object>
	//System.Threading.Tasks.Task`1<System.Object>
	//System.Threading.Tasks.Task`1<System.Byte>
	//System.Threading.Tasks.TaskCompletionSource`1<System.Byte>
	//System.Threading.Tasks.TaskCompletionSource`1<System.Object>
	//UnityEngine.Events.UnityAction`1<System.Object>
	//UnityEngine.Events.UnityAction`1<System.Single>
	//UnityEngine.Events.UnityEvent`1<System.Single>
	//UnityEngine.Events.UnityEvent`1<System.Object>
	//UnityEngine.Events.UnityEvent`1<UnityEngine.Vector2>
	// }}

	public void RefMethods()
	{
		// System.Object ComponentAutoBindTool::GetBindComponent<System.Object>(System.Int32)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder::AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask`1/Awaiter<System.Object>,cfg.Tables/<LoadAsync>d__49>(Cysharp.Threading.Tasks.UniTask`1/Awaiter<System.Object>&,cfg.Tables/<LoadAsync>d__49&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder::Start<cfg.Tables/<LoadAsync>d__49>(cfg.Tables/<LoadAsync>d__49&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder`1<System.Object>::AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask/Awaiter,Deer.ConfigManager/<LoadAllUserConfig>d__5>(Cysharp.Threading.Tasks.UniTask/Awaiter&,Deer.ConfigManager/<LoadAllUserConfig>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder`1<System.Object>::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Deer.ConfigManager/<ConfigLoader>d__6>(System.Runtime.CompilerServices.TaskAwaiter&,Deer.ConfigManager/<ConfigLoader>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder`1<System.Object>::Start<Deer.ConfigManager/<LoadAllUserConfig>d__5>(Deer.ConfigManager/<LoadAllUserConfig>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder`1<System.Object>::Start<Deer.ConfigManager/<ConfigLoader>d__6>(Deer.ConfigManager/<ConfigLoader>d__6&)
		// System.Object DG.Tweening.TweenSettingsExtensions::OnComplete<System.Object>(System.Object,DG.Tweening.TweenCallback)
		// System.Void GameFramework.Fsm.FsmState`1<System.Object>::ChangeState<System.Object>(GameFramework.Fsm.IFsm`1<System.Object>)
		// System.Object GameFramework.Fsm.IFsm`1<System.Object>::GetData<System.Object>(System.String)
		// System.Void GameFramework.Fsm.IFsm`1<System.Object>::SetData<System.Object>(System.String,System.Object)
		// System.Object GameFramework.GameFrameworkEntry::GetModule<System.Object>()
		// System.Void GameFramework.Network.INetworkChannel::Send<System.Object>(System.Object)
		// System.Object GameFramework.ReferencePool::Acquire<System.Object>()
		// System.String GameFramework.Utility/Text::Format<System.Object,System.Object>(System.String,System.Object,System.Object)
		// System.String GameFramework.Utility/Text::Format<System.Object,System.Object,System.Object>(System.String,System.Object,System.Object,System.Object)
		// System.String GameFramework.Utility/Text::Format<System.Object>(System.String,System.Object)
		// Google.Protobuf.FieldCodec`1<System.Object> Google.Protobuf.FieldCodec::ForMessage<System.Object>(System.UInt32,Google.Protobuf.MessageParser`1<System.Object>)
		// System.Object Google.Protobuf.ProtoPreconditions::CheckNotNull<System.Object>(System.Object,System.String)
		// System.Object System.Activator::CreateInstance<System.Object>()
		// System.Object[] System.Array::Empty<System.Object>()
		// System.Int32[] System.Array::Empty<System.Int32>()
		// System.Collections.Generic.List`1<System.Object> System.Linq.Enumerable::ToList<System.Object>(System.Collections.Generic.IEnumerable`1<System.Object>)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Byte>::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter`1<System.Byte>,TimerComponent/<FrameAsync>d__22>(System.Runtime.CompilerServices.TaskAwaiter`1<System.Byte>&,TimerComponent/<FrameAsync>d__22&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Byte>::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter`1<System.Byte>,TimerComponent/<OnceTimerAsync>d__21>(System.Runtime.CompilerServices.TaskAwaiter`1<System.Byte>&,TimerComponent/<OnceTimerAsync>d__21&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Byte>::Start<TimerComponent/<OnceTimerAsync>d__21>(TimerComponent/<OnceTimerAsync>d__21&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Byte>::Start<TimerComponent/<FrameAsync>d__22>(TimerComponent/<FrameAsync>d__22&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter`1<System.Byte>,UGFExtensions.CancellationToken/<CancelAfter>d__5>(System.Runtime.CompilerServices.TaskAwaiter`1<System.Byte>&,UGFExtensions.CancellationToken/<CancelAfter>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter`1<System.Object>,UGFExtensions.Texture.TextureSetComponent/<SetTextureByNetworkAsync>d__24>(System.Runtime.CompilerServices.TaskAwaiter`1<System.Object>&,UGFExtensions.Texture.TextureSetComponent/<SetTextureByNetworkAsync>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter`1<System.Object>,UGFExtensions.Texture.TextureSetComponent/<SetTextureByResourcesAsync>d__30>(System.Runtime.CompilerServices.TaskAwaiter`1<System.Object>&,UGFExtensions.Texture.TextureSetComponent/<SetTextureByResourcesAsync>d__30&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask`1/Awaiter<System.Object>,ConfigComponent/<LoadAllUserConfig>d__6>(Cysharp.Threading.Tasks.UniTask`1/Awaiter<System.Object>&,ConfigComponent/<LoadAllUserConfig>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter`1<System.Object>,UGFExtensions.SpriteCollection.SpriteCollectionComponent/<SetSpriteAsync>d__17>(System.Runtime.CompilerServices.TaskAwaiter`1<System.Object>&,UGFExtensions.SpriteCollection.SpriteCollectionComponent/<SetSpriteAsync>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<UGFExtensions.Texture.TextureSetComponent/<SetTextureByResourcesAsync>d__30>(UGFExtensions.Texture.TextureSetComponent/<SetTextureByResourcesAsync>d__30&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<UGFExtensions.SpriteCollection.SpriteCollectionComponent/<SetSpriteAsync>d__17>(UGFExtensions.SpriteCollection.SpriteCollectionComponent/<SetSpriteAsync>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<UGFExtensions.Texture.TextureSetComponent/<SetTextureByNetworkAsync>d__24>(UGFExtensions.Texture.TextureSetComponent/<SetTextureByNetworkAsync>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<UGFExtensions.CancellationToken/<CancelAfter>d__5>(UGFExtensions.CancellationToken/<CancelAfter>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<ConfigComponent/<LoadAllUserConfig>d__6>(ConfigComponent/<LoadAllUserConfig>d__6&)
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.Component::GetComponentInChildren<System.Object>()
		// System.Object[] UnityEngine.Component::GetComponentsInChildren<System.Object>()
		// System.Boolean UnityEngine.Component::TryGetComponent<System.Object>(System.Object&)
		// System.Object UnityEngine.GameObject::AddComponent<System.Object>()
		// System.Object UnityEngine.GameObject::GetComponent<System.Object>()
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object,UnityEngine.Transform,System.Boolean)
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object)
		// System.Object UnityExtension::GetOrAddComponent<System.Object>(UnityEngine.GameObject)
		// System.Boolean UnityGameFramework.Runtime.FsmComponent::DestroyFsm<System.Object>()
		// System.Object UnityGameFramework.Runtime.GameEntry::GetComponent<System.Object>()
		// System.Object UnityGameFramework.Runtime.Helper::CreateHelper<System.Object>(System.String,System.Object)
		// System.Void UnityGameFramework.Runtime.Log::Error<System.Object>(System.String,System.Object)
		// System.Void UnityGameFramework.Runtime.Log::Error<System.Object,System.Object>(System.String,System.Object,System.Object)
		// System.Void UnityGameFramework.Runtime.Log::Info<System.Object,System.Object>(System.String,System.Object,System.Object)
		// System.Void UnityGameFramework.Runtime.Log::Info<System.Object,System.Object,System.Object>(System.String,System.Object,System.Object,System.Object)
		// System.Void UnityGameFramework.Runtime.Log::Warning<System.Object,System.Object,System.Object>(System.String,System.Object,System.Object,System.Object)
		// System.Void UnityGameFramework.Runtime.Log::Warning<System.Object>(System.String,System.Object)
		// GameFramework.ObjectPool.IObjectPool`1<System.Object> UnityGameFramework.Runtime.ObjectPoolComponent::CreateMultiSpawnObjectPool<System.Object>(System.String,System.Single,System.Int32,System.Single,System.Int32)
		// GameFramework.ObjectPool.IObjectPool`1<System.Object> UnityGameFramework.Runtime.ObjectPoolComponent::CreateSingleSpawnObjectPool<System.Object>(System.String,System.Single,System.Int32,System.Single,System.Int32)
	}
}