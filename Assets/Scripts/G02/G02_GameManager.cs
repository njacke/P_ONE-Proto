using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02_GameManager : Singleton<G02_GameManager>
{
    [SerializeField] private float _padding = 1f;
    public float MinXBoundry { get; private set; }
    public float MaxXBoundry { get; private set; }
    public float MinYBoundry { get; private set; }
    public float MaxYBoundry { get; private set; }



    protected override void Awake() {
        base.Awake();
        SetUpMoveBoundries();
    }

    private void SetUpMoveBoundries() {
        Camera gameCamera = Camera.main;
        MinXBoundry = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + _padding;
        MaxXBoundry = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - _padding;
        MinYBoundry = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + _padding;
        MaxYBoundry = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - _padding;
    }
}
