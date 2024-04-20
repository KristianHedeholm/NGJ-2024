using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rewired player
    private Player _player;

    private enum EPlayerState
    {
        Normal,
        Zip, // Teleporting through light
    }

    private EPlayerState _prevState;
    private EPlayerState _state;

    private void Start()
    {
        _player = ReInput.players.GetPlayer(0);

        _prevState = _state = EPlayerState.Normal;
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case EPlayerState.Normal:
                _state = NormalUpdate();
                break;
            case EPlayerState.Zip:
                _state = ZipUpdate();
                break;
        }

        if (_state != _prevState)
        {
            // End-of-state calls
            switch (_prevState)
            {
                case EPlayerState.Normal:
                    NormalEnd();
                    break;
                case EPlayerState.Zip:
                    ZipEnd();
                    break;
            }

            // Start-of-state calls
            switch (_state)
            {
                case EPlayerState.Normal:
                    NormalStart();
                    break;
                case EPlayerState.Zip:
                    ZipStart();
                    break;
            }
        }
    }

    #region Player states

    private void NormalStart() { }

    private EPlayerState NormalUpdate()
    {
        return EPlayerState.Normal;
    }

    private void NormalEnd() { }

    private void ZipStart() { }

    private EPlayerState ZipUpdate()
    {
        return EPlayerState.Zip;
    }

    private void ZipEnd() { }

    #endregion
}
