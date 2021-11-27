using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.Databinding;
using Hugula.Databinding.Binder;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace Tests {
    public class ObservableCollectionTest {
        // A Test behaves as an ordinary method
        [Test]
        public void ObservableCollectionTestSimplePasses () {
            // ObservableCollection items;
            IList items;
            ObservableCollection<int> myarray = new ObservableCollection<int> ();
            myarray.CollectionChanged += (object sender, HugulaNotifyCollectionChangedEventArgs args) => {
                Debug.LogFormat ("Action={4},NewItems={0},OldItems={1},NewStartingIndex={2},OldStartingIndex={3}", args.NewItems, args.OldItems, args.NewStartingIndex, args.OldStartingIndex, args.Action);
            };

            myarray.Add (1);
            myarray.InsertRange (1, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            myarray.ReplaceRange (2, new int[] { 13, 14, 15, 16 });
            myarray.Remove (10);
            myarray.RemoveRange (new int[] { 8, 9 });
            myarray.RemoveAt (6);
            // myarray.
            myarray.Clear ();

            /**
            Action=Add,NewItems=System.Collections.ArrayList+ReadOnlyList,OldItems=,NewStartingIndex=0,OldStartingIndex=-1
            Action=Add,NewItems=System.Collections.ArrayList+ReadOnlyList,OldItems=,NewStartingIndex=1,OldStartingIndex=-1
            Action=Replace,NewItems=System.Collections.ArrayList+ReadOnlyList,OldItems=System.Collections.ArrayList+ReadOnlyList,NewStartingIndex=2,OldStartingIndex=2
            Action=Remove,NewItems=,OldItems=System.Collections.ArrayList+ReadOnlyList,NewStartingIndex=-1,OldStartingIndex=9
            Action=Remove,NewItems=,OldItems=System.Collections.ArrayList+ReadOnlyList,NewStartingIndex=-1,OldStartingIndex=-1
            Action=Remove,NewItems=,OldItems=System.Collections.ArrayList+ReadOnlyList,NewStartingIndex=-1,OldStartingIndex=6
            Action=Reset,NewItems=,OldItems=,NewStartingIndex=-1,OldStartingIndex=-1
            **/
   
            items = (IList) myarray;

            Debug.LogFormat ("myarray={0},items={1} ,equals({2})", myarray, items, myarray.Equals (items));

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ObservableCollectionTestWithEnumeratorPasses () {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}