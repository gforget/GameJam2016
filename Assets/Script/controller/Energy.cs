using UnityEngine;
using System.Collections;

public class Energy : MonoBehaviour {

    public float totalEnergy = 100.0f;
    public float normalEnergyLost = 0.25f;
    public float boostEnergyLost = 0.55f;

    public float currentEnergy
    {
        get
        {
            return _currentEnergy;
        }

        set
        {
            _currentEnergy = value;

            if (_currentEnergy > totalEnergy)
            {
                _currentEnergy = totalEnergy;
            }

            if (_currentEnergy < 0f)
            {
                _currentEnergy = 0f;
            }
        }
    }

    private float _currentEnergy = 100.0f;

    private float energyToLose = 0f;
    // Use this for initialization

    public delegate void DeathEvent();
    public event DeathEvent OnDeath;

    void Start () {
        currentEnergy = totalEnergy;
        energyToLose = normalEnergyLost;
        StartCoroutine(loseEnergy());
    }

    public void toggleBoostMode(bool boostMode) 
    {
        if (boostMode)
        {
            energyToLose = boostEnergyLost;
        }
        else
        {
            energyToLose = normalEnergyLost;
        }
    }

    WaitForSeconds waitTimer = new WaitForSeconds(0.2f);
    IEnumerator loseEnergy()
    {
        while (currentEnergy > 0f)
        {
            currentEnergy -= energyToLose;
            yield return waitTimer;   
        }

        if (OnDeath != null) OnDeath();
    }
}
