﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class GridUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    , IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Enter&&Exit
    public static Action<Transform> OnEnter;
    public static Action OnExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "Grid")
        {
            if (OnEnter != null)
                OnEnter(transform);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "Grid")
        {
            if (OnExit != null)
                OnExit();
        }
    }
    #endregion


    public static Action<Transform> OnLeftBeginDrag;
    public static Action<Transform, Transform> OnLeftEndDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (OnLeftBeginDrag != null)
                OnLeftBeginDrag(transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (OnLeftEndDrag != null)
            {
                if (eventData.pointerEnter == null)
                    OnLeftEndDrag(transform, null);
                else
                    OnLeftEndDrag(transform, eventData.pointerEnter.transform);
            }
        }
    }

}
