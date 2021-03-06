﻿using UnityEngine;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {
    public Wheel[] leftWheels;
    public Wheel[] rightWheels;
    public Wheel[] centerWheel;

    public float motorScaler;
    public float newSpeed = 1.0f;
    public float intakeScaler;

    public int numBalls;
    public int MAX_BALLS = 50;
    public bool holdingGear;

    public Gripper gripper;
    public IntakeArm intakeArm;
    public Launcher launcher;
    public Launcher launcher2;
    public Dictionary<ActuatedDefense, int> actuatedDefenses = new Dictionary<ActuatedDefense, int>();
    public Intake intake;

    public GUIText scoreText;
    private int score;
    private int ballScored;
    private int highScored;

    private float headingPrev = 0.0f;

    void Update()
    {
        if (transform.position.z > 35)
        {
            newSpeed = 1.5f;
        }

        else {
            newSpeed = 1;
        }
        var current = Heading;
        var diff = Mathf.DeltaAngle(current, headingPrev);
        Gyro += diff;
        headingPrev = current;
    }

    public void Actuate()
    {
        foreach (var d in actuatedDefenses)
        {
            if (d.Value > 0)
            {
                d.Key.Actuate(this);
            }
        }
    }

    public void SetMotors(float left, float right, float center)
    {
        //Debug.Log("Left: " + left + "Right: " + right + "Center: " + center);

        foreach (var h in leftWheels)
        {
            h.RunJoint(motorScaler * left * newSpeed);
        }
        foreach (var h in rightWheels)
        {
            h.RunJoint(motorScaler * right * newSpeed);
        }
        foreach (var h in centerWheel)
        {
            h.RunJoint(motorScaler * center * newSpeed);
        }
    }

    public void SetIntake(float speed)
    {
        intake.setSpeed(speed);
    }

    public void incrementScore(int add)
    {
        score += add;
        updateGUI();
    }

    private void updateGUI()
    {
        scoreText.text = "Holding Gear: " + holdingGear + "\nGears Scored: " + score + "\nBalls Holding: " + numBalls + "\nLow Goal Score: " + ballScored + "\nHigh Goal Score: " + highScored;
    }

    public void SetGripper(bool state)
    {
        if (state)
            gripper.MoveOut();
        else
            gripper.MoveIn();
    }

    public void SetIntakeArm(float speed)
    {
        intakeArm.RunJoint(intakeScaler * speed);
    }

    public void addBalls(int num)
    {
        numBalls += num;
        if (numBalls > MAX_BALLS)
            numBalls = MAX_BALLS;
        updateGUI();
    }

    public void fillHopper(bool full)
    {
        if (full)
            numBalls = MAX_BALLS;
        else
            numBalls = 0;
        updateGUI();
    }

    public float ShootPower
    {
        get
        {
            return launcher.ShootPower;
        }
        set
        {
            launcher.ShootPower = value;
        }
    }

    public void setHoldingGear(bool holding)
    {
        holdingGear = holding;
        updateGUI();
    }

    public void Launch()
    {
        launcher.Launch();
    }

    public void Launch2()
    {
        launcher2.Launch();
    }

    public int LeftEncoder
    {
        get
        {
            if (leftWheels.Length == 0)
                return 0;
            return leftWheels[0].Encoder;
        }
    }

    public int RightEncoder
    {
        get
        {
            if (rightWheels.Length == 0)
                return 0;
            return rightWheels[0].Encoder;
        }
    }

    public int CenterEncoder
    {
        get
        {
            if (centerWheel.Length == 0)
                return 0;
            return centerWheel[0].Encoder;
        }
    }

    static float Angle360(Vector3 v1, Vector3 v2, Vector3 n)
    {
        //  Acute angle [0,180]
        float angle = Vector3.Angle(v1, v2);

        //  -Acute angle [180,-179]
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(v1, v2)));
        return angle * sign;
    }

    public float Heading
    {
        get
        {
            return Angle360(Vector3.forward, transform.forward, Vector3.up);
        }
    }

    public float Gyro {
        get;
        private set;
    }

    public bool GripperState
    {
        get
        {
            return gripper.state;
        }
    }

    public bool BallPresence
    {
        get
        {
            return gripper.payload.Get() != null;
        }
    }

    public int getNumBalls()
    {
        return numBalls;
    }

    public void addBallScored(int numScored)
    {
        ballScored += numScored;
        updateGUI();
    }

    public void scoreAllBalls()
    {
        addBallScored(numBalls);
        numBalls = 0;
    }

    public void addHighScored(int numScored)
    {
        highScored += numScored;
        updateGUI();
    }
}

