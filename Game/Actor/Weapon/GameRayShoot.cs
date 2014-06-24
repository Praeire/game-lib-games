﻿using UnityEngine;
using System.Collections;

public class GameRayShoot : GameDamageBase {

    public int Range = 10000;
    public Vector3 AimPoint;
    public GameObject Explosion;
    public float LifeTime = 1;
    private LineRenderer trail;
    
    void Start() {
        trail = this.gameObject.GetComponent<LineRenderer>();
        RaycastHit hit;
        GameObject explosion = null;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, Range)) {
            AimPoint = hit.point;
            if (Explosion != null) {
                explosion = (GameObject)GameObject.Instantiate(Explosion, AimPoint, this.transform.rotation);
            }
        }
        else {
            AimPoint = this.transform.forward * Range;
            explosion = (GameObject)GameObject.Instantiate(Explosion, AimPoint, this.transform.rotation);
            
        }
        if (explosion) {
            GameDamageBase dmg = explosion.GetComponent<GameDamageBase>();
            if (dmg) {
                dmg.TargetTag = TargetTag;  
            }
        }
        if (trail) {
            trail.SetPosition(0, this.transform.position);
            trail.SetPosition(1, AimPoint);
        }
        Destroy(this.gameObject, LifeTime);
    }

    void Update() {
        
    }
}