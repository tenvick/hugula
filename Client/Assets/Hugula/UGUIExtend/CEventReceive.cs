using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
/**IPointerEnterHandler - OnPointerEnter - Called when a pointer enters the object•IPointerExitHandler - OnPointerExit - Called when a pointer exits the object•IPointerDownHandler - OnPointerDown - Called when a pointer is pressed on the object•IPointerUpHandler - OnPointerUp - Called when a pointer is released (called on the original the pressed object)•IPointerClickHandler - OnPointerClick - Called when a pointer is pressed and released on the same object•IInitializePotentialDragHandler - OnInitializePotentialDrag - Called when a drag target is found, can be used to initialise values•IBeginDragHandler - OnBeginDrag - Called on the drag object when dragging is about to begin•IDragHandler - OnDrag - Called on the drag object when a drag is happening•IEndDragHandler - OnEndDrag - Called on the drag object when a drag finishes•IDropHandler - OnDrop - Called on the object where a drag finishes•IScrollHandler - OnScroll - Called when a mouse wheel scrolls•IUpdateSelectedHandler - OnUpdateSelected - Called on the selected object each tick•ISelectHandler - OnSelect - Called when the object becomes the selected object•IDeselectHandler - OnDeselect - Called on the selected object becomes deselected•IMoveHandler - OnMove - Called when a move event occurs (left, right, up, down, ect)•ISubmitHandler - OnSubmit - Called when the submit button is pressed•ICancelHandler - OnCancel - Called when the cancel button is pressed ***/

namespace Hugula.UGUIExtend
{
    /// <summary>
    /// 事件接受并抛给lua
    /// </summary>
    [SLua.CustomLuaClass]
    public class CEventReceive : MonoBehaviour//, ISelectHandler//,IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IDropHandler, ICancelHandler
    {

        public void OnPointerDown(BaseEventData eventData)
        {
            var g = eventData.selectedObject;
            UGUIEvent.onPressHandle(g, eventData);
        }

        public void OnPointerUp(BaseEventData eventData)
        {
            var g = eventData.selectedObject;
            UGUIEvent.onPressHandle(g, eventData);
        }

        //public void OnBeginDrag(BaseEventData eventData)
        //{
        //    var g = EventSystem.current.currentSelectedGameObject;
        //    UGUIEvent.onCancelHandle(g, eventData);
        //}

        public void OnDrag(BaseEventData eventData)
        {
            var g = eventData.selectedObject;
            PointerEventData ped = eventData as PointerEventData;
            UGUIEvent.onDragHandle(g, ped.delta);
        }

        public void OnDrop(BaseEventData eventData)
        {
            var g = eventData.selectedObject;
            UGUIEvent.onDropHandle(g, eventData);
        }

        public void OnPointerClick(BaseEventData eventData)
        {
            //PointerEventData ped = eventData as PointerEventData;
            var g = eventData.selectedObject;
            UGUIEvent.onClickHandle(g, eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            var g = eventData.selectedObject;
            UGUIEvent.onSelectHandle(g, eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            var g = eventData.selectedObject;
            UGUIEvent.onCancelHandle(g, eventData);
        }

        public void OnButtonClick(MonoBehaviour arg)
        {
            var g = EventSystem.current.currentSelectedGameObject;
            UGUIEvent.onClickHandle(g, arg); //Debug.Log("you are click "+g);
        }

        public void OnButtonClick(Object arg)
        {
            var g = EventSystem.current.currentSelectedGameObject;
            UGUIEvent.onClickHandle(g, arg); //Debug.Log("you are click "+g);
        }


        public void OnCustomerEvent(MonoBehaviour arg)
        {
            var g = EventSystem.current.currentSelectedGameObject;
            UGUIEvent.onCustomerHandle(g, arg); //Debug.Log("you are click "+g);
        }

        public void OnCustomerEvent(Object arg)
        {
            var g = EventSystem.current.currentSelectedGameObject;
            UGUIEvent.onCustomerHandle(g, arg); //Debug.Log("you are click "+g);
        }
    }
}