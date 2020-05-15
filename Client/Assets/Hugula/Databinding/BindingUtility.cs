using System;
using System.Collections;
using System.Collections.Specialized;
using Hugula.Databinding;
using Hugula.Databinding.Binder;

namespace Hugula.Databinding {
	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public static class BindingUtility {
		public static void SetContextByINotifyTable (BindableObject bindable, INotifyTable notify) {
			bindable.context = notify;
		}

		public static void SetContextByIList (BindableObject bindable, IList list) {
			bindable.context = list;
		}
		public static void SetContextByINotifyPropertyChanged (BindableObject bindable, INotifyPropertyChanged notify) {
			bindable.context = notify;
		}

		public static NotifyCollectionChangedEventArgs CreateCollectionArgsChangedItems (NotifyCollectionChangedAction action, IList changedItems) {
			return new NotifyCollectionChangedEventArgs (action, changedItems);
		}
		public static NotifyCollectionChangedEventArgs CreateCollectionArgsChangedItem (NotifyCollectionChangedAction action, object changedItem) {
			return new NotifyCollectionChangedEventArgs (action, changedItem);
		}

		public static NotifyCollectionChangedEventArgs CreateCollectionArgsNewItemsOldItems (NotifyCollectionChangedAction action, IList newItems, IList oldItems) {
			return new NotifyCollectionChangedEventArgs (action, newItems, oldItems);
		}

		public static NotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemsStartingIndex (NotifyCollectionChangedAction action, IList changedItems, int startingIndex) {
			return new NotifyCollectionChangedEventArgs (action, changedItems, startingIndex);
		}

		public static NotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemIndex (NotifyCollectionChangedAction action, object changedItem, int index) {
			return new NotifyCollectionChangedEventArgs (action, changedItem, index);
		}
		public static NotifyCollectionChangedEventArgs CreateCollectionArgsNewItemOldItem (NotifyCollectionChangedAction action, object newItem, object oldItem) {
			return new NotifyCollectionChangedEventArgs (action, newItem, oldItem);

		}
		public static NotifyCollectionChangedEventArgs CreateCollectionArgsNewItemsOldItemsStartingIndex (NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex) {
			return new NotifyCollectionChangedEventArgs (action, newItems, oldItems, startingIndex);

		}
		public static NotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemsIndexOldIndex (NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex) {
			return new NotifyCollectionChangedEventArgs (action, changedItems, index, oldIndex);
		}
		public static NotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemIndexOldIndex (NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex) {
			return new NotifyCollectionChangedEventArgs (action, changedItem, index, oldIndex);
		}

		public static NotifyCollectionChangedEventArgs CreateCollectionArgsNewItemOldItemIndex (NotifyCollectionChangedAction action, object newItem, object oldItem, int index) {
			return new NotifyCollectionChangedEventArgs (action, newItem, oldItem, index);

		}

		public static BindableContainer GetBindableContainer(UnityEngine.GameObject obj)
		{
			return obj.GetComponent<BindableContainer>();
		}

		public static BindableObject GetBindableObject(UnityEngine.GameObject obj)
		{
			return obj.GetComponent<BindableObject>();
		}

		public static object ConvertToNotifyTable(INotifyPropertyChanged table)
		{
			UnityEngine.Debug.LogWarning(" ConvertToNotifyTable:"+table);
			return table;
		}

	}
}