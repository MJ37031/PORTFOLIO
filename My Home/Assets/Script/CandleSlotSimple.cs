using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class CandleSlotSimple : MonoBehaviour
{
    [Tooltip("이 소켓에 맞는(들어와야 하는) 촛대")]
    public CandleID expected; // 해당 위치의 정답 촛대(씬 인스턴스) 드래그

    public bool IsCorrect { get; private set; }
    public bool HasCandle => _current != null;

    public UnityEvent onStateChanged; // 퍼즐 매니저에서 묶어 쓰기 좋음

    XRSocketInteractor _socket;
    CandleID _current;

    void Awake() => _socket = GetComponent<XRSocketInteractor>();

    void OnEnable()
    {
        _socket.selectEntered.AddListener(OnEnter);
        _socket.selectExited.AddListener(OnExit);
    }

    void OnDisable()
    {
        _socket.selectEntered.RemoveListener(OnEnter);
        _socket.selectExited.RemoveListener(OnExit);
    }

    void OnEnter(SelectEnterEventArgs args)
    {
        _current = args.interactableObject.transform.GetComponentInParent<CandleID>();
        Evaluate();
    }

    void OnExit(SelectExitEventArgs args)
    {
        _current = null;
        Evaluate();
    }

    void Evaluate()
    {
        IsCorrect = (_current != null && expected != null && ReferenceEquals(_current, expected));
        onStateChanged?.Invoke();
    }
}
