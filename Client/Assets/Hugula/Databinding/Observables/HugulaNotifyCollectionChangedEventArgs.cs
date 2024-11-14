using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System;

namespace Hugula.Databinding
{



    public class HugulaNotifyCollectionChangedEventArgs : EventArgs
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a reset change.
        /// </summary>
        /// <param name="action">The action that caused the event (must be Reset).</param>
        public HugulaNotifyCollectionChangedEventArgs()
        {

        }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a reset change.
        /// </summary>
        /// <param name="action">The action that caused the event (must be Reset).</param>
        public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            if (action != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Reset), "action");

            InitializeAdd(action, 0, -1);
        }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a one-item change.
        /// </summary>
        /// <param name="action">The action that caused the event; can only be Reset, Add or Remove action.</param>
        /// <param name="changedItem">The item affected by the change.</param>
        public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int changedItem)
        {
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
                    && (action != NotifyCollectionChangedAction.Reset))
                throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

            if (action == NotifyCollectionChangedAction.Reset)
            {
                //if (changedItem != null)
                //    throw new ArgumentException(SR.GetString(SR.ResetActionRequiresNullItem), "action");

                InitializeAdd(action, changedItem, -1);
            }
            else
            {
                InitializeAddOrRemove(action, changedItem, -1);// new object[] { changedItem }, -1);
            }
        }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a one-item change.
        /// </summary>
        /// <param name="action">The action that caused the event.</param>
        /// <param name="changedItem">The item affected by the change.</param>
        /// <param name="index">The index where the change occurred.</param>
        public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int changedItem, int index)
        {
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
                    && (action != NotifyCollectionChangedAction.Reset))
                throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

            if (action == NotifyCollectionChangedAction.Reset)
            {
                //if (changedItem != null)
                //    throw new ArgumentException(SR.GetString(SR.ResetActionRequiresNullItem), "action");
                if (index != -1)
                    throw new ArgumentException(SR.GetString(SR.ResetActionRequiresIndexMinus1), "action");

                InitializeAdd(action, 0, -1);
            }
            else
            {
                InitializeAddOrRemove(action, changedItem, index);// new object[] { changedItem }, index);
            }
        }

        // /// <summary>
        // /// Construct a NotifyCollectionChangedEventArgs that describes a multi-item change.
        // /// </summary>
        // /// <param name="action">The action that caused the event.</param>
        // /// <param name="changedItems">The items affected by the change.</param>
        // public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int changedItems)
        // {
        //     if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
        //             && (action != NotifyCollectionChangedAction.Reset))
        //         throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

        //     if (action == NotifyCollectionChangedAction.Reset)
        //     {
        //         if (changedItems != 0)
        //             throw new ArgumentException(SR.GetString(SR.ResetActionRequiresNullItem), "action");

        //         InitializeAdd(action, 0, -1);
        //     }
        //     else
        //     {
        //         if (changedItems == 0)
        //             throw new ArgumentNullException("changedItems");

        //         InitializeAddOrRemove(action, changedItems, -1);
        //     }
        // }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a multi-item change (or a reset).
        /// </summary>
        /// <param name="action">The action that caused the event.</param>
        /// <param name="changedItems">The items affected by the change.</param>
        // /// <param name="startingIndex">The index where the change occurred.</param>
        // public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int changedItems, int startingIndex)
        // {
        //     if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
        //             && (action != NotifyCollectionChangedAction.Reset))
        //         throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

        //     if (action == NotifyCollectionChangedAction.Reset)
        //     {
        //         if (changedItems != 0)
        //             throw new ArgumentException(SR.GetString(SR.ResetActionRequiresNullItem), "action");
        //         if (startingIndex != -1)
        //             throw new ArgumentException(SR.GetString(SR.ResetActionRequiresIndexMinus1), "action");

        //         InitializeAdd(action, 0, -1);
        //     }
        //     else
        //     {
        //         if (changedItems == 0)
        //             throw new ArgumentNullException("changedItems");
        //         if (startingIndex < -1)
        //             throw new ArgumentException(SR.GetString(SR.IndexCannotBeNegative), "startingIndex");

        //         InitializeAddOrRemove(action, changedItems, startingIndex);
        //     }
        // }

        // /// <summary>
        // /// Construct a NotifyCollectionChangedEventArgs that describes a one-item Replace event.
        // /// </summary>
        // /// <param name="action">Can only be a Replace action.</param>
        // /// <param name="newItem">The new item replacing the original item.</param>
        // /// <param name="oldItem">The original item that is replaced.</param>
        // public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int newItem, int oldItem)
        // {
        //     if (action != NotifyCollectionChangedAction.Replace)
        //         throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Replace), "action");

        //     InitializeMoveOrReplace(action, newItem, oldItem, -1, -1);// new object[] { newItem }, new object[] { oldItem }, -1, -1);
        // }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a one-item Replace event.
        /// </summary>
        /// <param name="action">Can only be a Replace action.</param>
        /// <param name="newItem">The new item replacing the original item.</param>
        /// <param name="oldItem">The original item that is replaced.</param>
        /// <param name="index">The index of the item being replaced.</param>
        public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int newItem, int oldItem, int index)
        {
            if (action != NotifyCollectionChangedAction.Replace)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Replace), "action");

            int oldStartingIndex = index;

            InitializeMoveOrReplace(action, newItem, oldItem, index, oldStartingIndex);  //  new object[] { newItem }, new object[] { oldItem }, index, oldStartingIndex);
        }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a multi-item Replace event.
        /// </summary>
        /// <param name="action">Can only be a Replace action.</param>
        /// <param name="newItems">The new items replacing the original items.</param>
        /// <param name="oldItems">The original items that are replaced.</param>
        // public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int newItems, int oldItems)
        // {
        //     if (action != NotifyCollectionChangedAction.Replace)
        //         throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Replace), "action");
        //     if (newItems == null)
        //         throw new ArgumentNullException("newItems");
        //     if (oldItems == null)
        //         throw new ArgumentNullException("oldItems");

        //     InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
        // }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a multi-item Replace event.
        /// </summary>
        /// <param name="action">Can only be a Replace action.</param>
        /// <param name="newItems">The new items replacing the original items.</param>
        /// <param name="oldItems">The original items that are replaced.</param>
        /// <param name="startingIndex">The starting index of the items being replaced.</param>
        // public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int newItems, int oldItems, int startingIndex)
        // {
        //     if (action != NotifyCollectionChangedAction.Replace)
        //         throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Replace), "action");
        //     if (newItems == null)
        //         throw new ArgumentNullException("newItems");
        //     if (oldItems == null)
        //         throw new ArgumentNullException("oldItems");

        //     InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
        // }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a one-item Move event.
        /// </summary>
        /// <param name="action">Can only be a Move action.</param>
        /// <param name="changedItem">The item affected by the change.</param>
        /// <param name="index">The new index for the changed item.</param>
        /// <param name="oldIndex">The old index for the changed item.</param>
        // public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int changedItems, int index, int oldIndex)
        // {
        //     if (action != NotifyCollectionChangedAction.Move)
        //         throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Move), "action");
        //     if (index < 0)
        //         throw new ArgumentException(SR.GetString(SR.IndexCannotBeNegative), "index");

        //     // object[] changedItems = new object[] { changedItem };
        //     InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        // }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs that describes a multi-item Move event.
        /// </summary>
        /// <param name="action">The action that caused the event.</param>
        /// <param name="changedItems">The items affected by the change.</param>
        /// <param name="index">The new index for the changed items.</param>
        /// <param name="oldIndex">The old index for the changed items.</param>
        // public HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int changedItems, int index, int oldIndex)
        // {
        //     if (action != NotifyCollectionChangedAction.Move)
        //         throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Move), "action");
        //     if (index < 0)
        //         throw new ArgumentException(SR.GetString(SR.IndexCannotBeNegative), "index");

        //     InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        // }

        /// <summary>
        /// Construct a NotifyCollectionChangedEventArgs with given fields (no validation). Used by WinRT marshaling.
        /// </summary>
        internal HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, int newItems, int oldItems, int newIndex, int oldIndex)
        {
            _action = action;

            _newItems = newItems; //(newItems == null) ? null : ArrayList.ReadOnly(newItems);
            _oldItems = oldItems;//(oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
            _newStartingIndex = newIndex;
            _oldStartingIndex = oldIndex;
        }

        internal void InitializeAddOrRemove(NotifyCollectionChangedAction action, int changedItems, int startingIndex)
        {
            if (action == NotifyCollectionChangedAction.Add)
                InitializeAdd(action, changedItems, startingIndex);
            else if (action == NotifyCollectionChangedAction.Remove)
                InitializeRemove(action, changedItems, startingIndex);
            else
                UnityEngine.Debug.LogError(String.Format("Unsupported action: {0}", action.ToString()));
        }

        internal void InitializeAdd(NotifyCollectionChangedAction action, int newItems, int newStartingIndex)
        {
            _action = action;
            _newItems = newItems;// (newItems == null) ? null : ArrayList.ReadOnly(newItems);
            _newStartingIndex = newStartingIndex;
        }

        internal void InitializeRemove(NotifyCollectionChangedAction action, int oldItems, int oldStartingIndex)
        {
            _action = action;
            _oldItems = oldItems;// (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
            _oldStartingIndex = oldStartingIndex;
        }

        internal void InitializeMoveOrReplace(NotifyCollectionChangedAction action, int newItems, int oldItems, int startingIndex, int oldStartingIndex)
        {
            InitializeAdd(action, newItems, startingIndex);
            InitializeRemove(action, oldItems, oldStartingIndex);
        }

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        /// <summary>
        /// The action that caused the event.
        /// </summary>
        public NotifyCollectionChangedAction Action
        {
            get { return _action; }
        }

        /// <summary>
        /// The items affected by the change.
        /// </summary>
        public int NewItems
        {
            get { return _newItems; }
        }

        /// <summary>
        /// The old items affected by the change (for Replace events).
        /// </summary>
        public int OldItems
        {
            get { return _oldItems; }
        }

        /// <summary>
        /// The index where the change occurred.
        /// </summary>
        public int NewStartingIndex
        {
            get { return _newStartingIndex; }
        }

        /// <summary>
        /// The old index where the change occurred (for Move events).
        /// </summary>
        public int OldStartingIndex
        {
            get { return _oldStartingIndex; }
        }

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        private NotifyCollectionChangedAction _action;
        private int _newItems, _oldItems;
        private int _newStartingIndex = -1;
        private int _oldStartingIndex = -1;

        public void Release()
        {
            _action = NotifyCollectionChangedAction.Reset;
            _newItems = 0;
            _oldItems = 0;
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
        }
    }

    internal sealed class SR
    {
        internal const string WrongActionForCtor = "WrongActionForCtor";

        internal const string MustBeResetAddOrRemoveActionForCtor = "MustBeResetAddOrRemoveActionForCtor";

        internal const string ResetActionRequiresNullItem = "ResetActionRequiresNullItem";

        internal const string ResetActionRequiresIndexMinus1 = "ResetActionRequiresIndexMinus1";
        internal const string IndexCannotBeNegative = "IndexCannotBeNegative";


        public static string GetString(string name)
        {
            return name;
        }

        public static string GetString(string name, object arg)
        {
            return name + arg;
        }
    }
}