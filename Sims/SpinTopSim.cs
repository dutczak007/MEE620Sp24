//============================================================================
// SpinTopSim.cs   Class for creating a simulation of a spinning top.
//============================================================================
using System;
using Godot;

public class SpinTopSim : Simulator
{
    enum SimMode
    {
        BodyFixed,
        LeanFrame,
    }
    SimMode simMode;

    // physical parameters
    double h;    // distance of CG from contact rotation point
    double m;   // mass of top
    double IGa;  // moment of inertia about its spin axis
    double IGp;  // moment of inertia about its CG, perpendicular to spin axis
    double ICp; // moment of inertia about contact point, perp to spin axis

    double ke;   // kinetic energy to be calculated by auxFunc
    double pe;   // potential energy to be calculated by auxFunc
    double angMoY; //vertical component of angular momentum calc by auxFun 
    private Action AuxFunc;  // delegate for aux

    //------------------------------------------------------------------------
    // Constructor      [STUDENTS: DO NOT CHANGE THIS FUNCTION]
    //------------------------------------------------------------------------
    public SpinTopSim() : base(6)
    {
        m = 4.0;
        h = 0.8;
        double r = 0.5;
        IGa = 0.5*m*r*r;
        IGp = 0.5*IGa;
        ICp = IGp + m*h*h;

        ke = pe = angMoY = 0.0;

        // Default initial conditions
        x[0] = 0.0;    // generalized coord: precession angle psi
        x[1] = Math.PI/6.0;    // generalized coord: lean angle phi
        x[2] = 0.0;    // generalized coord: spin angle theta
        x[3] = 0.0;    // gen speed: omegaX or psiDot, depending on sim type
        x[4] = 5.0;    // gen speed: omegaY or phiDot, depending on sim type
        x[5] = 0.0;    // gen speed: omegaZ or thetadot, dep on sim type

        SetRHSFunc(RHSFuncSpinTopBody);
        AuxFunc = AuxFuncBody;
    }

    //------------------------------------------------------------------------
    // RHSFuncSpinTopBody:  Evaluates the right sides of the differential
    //                 equations for the spinning top (body frame)
    //------------------------------------------------------------------------
    private void RHSFuncSpinTopBody(double[] xx, double t, double[] ff)
    {
        double psi = xx[0];
        double phi = xx[1];
        double theta = xx[2];
        double omegaX = xx[3];
        double omegaY = xx[4];
        double omegaZ = xx[5];

        //double cosPhi = Math.Cos(phi);
        double sinPhi = Math.Sin(phi);
        double tanPhi = Math.Tan(phi);
        double cosTheta = Math.Cos(theta);
        double sinTheta = Math.Sin(theta);

        // Evaluate right sides of differential equations of motion
        // ##### You will need to provide these ###### //
        ff[0] = cosTheta*omegaX/sinPhi + sinTheta*omegaZ/sinPhi;   // time deriv of state psi
        ff[1] = -sinTheta*omegaX + cosTheta*omegaZ;   // time deriv os state phi
        ff[2] = -cosTheta/tanPhi*omegaX + omegaY - sinTheta*omegaZ/tanPhi;   // time deriv os state theta
        ff[3] = ((IGa-ICp)*omegaY*omegaZ - m*g*h*sinPhi*sinTheta)/ICp;   // time deriv os state omegaX
        ff[4] = 0.0;   // time deriv os state omegaY
        ff[5] = ((ICp-IGa)*omegaX*omegaY + m*g*h*sinPhi*cosTheta)/ICp;   // time deriv os state omegaZ
    }

    //------------------------------------------------------------------------
    // CalcKineticEnergyBody
    //------------------------------------------------------------------------
    private double CalcKineticEnergyBody()
    {
        //******* Students write this function ********

        return 0.0;
    }

    //------------------------------------------------------------------------
    // CalcPotentialEnergyBody
    //------------------------------------------------------------------------
    private double CalcPotentialEnergyBody()
    {
        //******* Students write this function ********

        return 0.0;
    }

    //------------------------------------------------------------------------
    // CalcAngMoVertBody
    //------------------------------------------------------------------------
    private double CalcAngMoVertBody()
    {
        //******* Students write this function ********
        
        return 0.0;
    }

    //------------------------------------------------------------------------
    // AuxFuncBody: This function is used to calculate angular velocity, 
    //     angular momentum, and other quantities of interest
    //------------------------------------------------------------------------
    private void AuxFuncBody()
    {
        ke = CalcKineticEnergyBody();
        pe = CalcPotentialEnergyBody();
        angMoY = CalcAngMoVertBody();
    }

    //------------------------------------------------------------------------
    // RHSFuncSpinTopLean:  Evaluates the right sides of the differential
    //                 equations for the spinning top (lean frame)
    //------------------------------------------------------------------------
    private void RHSFuncSpinTopLean(double[] xx, double t, double[] ff)
    {
        double psi = xx[0];
        double phi = xx[1];
        double theta = xx[2];
        double psiDot = xx[3];
        double phiDot = xx[4];
        double thetaDot = xx[5];

        double cosPhi = Math.Cos(phi);
        double sinPhi = Math.Sin(phi);
        double tanPhi = Math.Tan(phi);

        // Evaluate right sides of differential equations of motion
        // ##### You will need to provide these ###### //
        ff[0] = 0.0;   // time deriv of state psi
        ff[1] = 0.0;   // time deriv of state phi
        ff[2] = 0.0;   // time deriv of state theta
        ff[3] = 0.0;   // time deriv of state psiDot
        ff[4] = 0.0;   // time deriv of state phiDot
        ff[5] = 0.0;   // time deriv of state thetaDot

    }

    //------------------------------------------------------------------------
    // AuxFuncLean: This function is used to calculate angular velocity, 
    //     angular momentum, and other quantities of interest
    //------------------------------------------------------------------------
    private void AuxFuncLean()
    {
        ke = 1.1;
        pe = 1.2;
        angMoY = 1.3;
    }

    //------------------------------------------------------------------------
    // ResetIC
    //------------------------------------------------------------------------
    public void ResetIC(double ln, double sr)
    {
        if(simMode == SimMode.BodyFixed){
            x[0] = 0.0;    // generalized coord: precession angle psi
            x[1] = ln;     // generalized coord: lean angle phi
            x[2] = 0.0;    // generalized coord: spin angle theta
            x[3] = 0.0;    // generalized speed: omegaX
            x[4] = sr;     // generalized speed: omegaY (spin rate)
            x[5] = 0.0;    // generalized speed: omegaZ
        }
        if(simMode == SimMode.LeanFrame){
            x[0] = 0.0;    // generalized coord: precession angle psi
            x[1] = ln;     // generalized coord: lean angle phi
            x[2] = 0.0;    // generalized coord: spin angle theta
            x[3] = 0.0;    // generalized speed: psiDot
            x[4] = 0.0;    // generalized speed: phiDot
            x[5] = sr;     // generalized speed: thetaDot
        }
    }

    //------------------------------------------------------------------------
    // SwitchModelBody: Use the body fixed model to perform calculations
    // [STUDENTS: DO NOT MODIFY THIS FUNCTION]
    //------------------------------------------------------------------------
    public void SwitchModelBody()
    {
        simMode = SimMode.BodyFixed;
        SetRHSFunc(RHSFuncSpinTopBody);
        AuxFunc = AuxFuncBody;
    }

    //------------------------------------------------------------------------
    // SwitchModelLean: Use the lean frame model to perform calculations
    // [STUDENTS: DO NOT MODIFY THIS FUNCTION]
    //------------------------------------------------------------------------
    public void SwitchModelLean()
    {
        simMode = SimMode.LeanFrame;
        SetRHSFunc(RHSFuncSpinTopLean);
        AuxFunc = AuxFuncLean;
    }

    //------------------------------------------------------------------------
    // PostProcess
    //------------------------------------------------------------------------
    public void PostProcess()
    {
        AuxFunc();
    }

    //------------------------------------------------------------------------
    // getters & setters
    //------------------------------------------------------------------------

    // PrecessionAngle, psi
    public double PrecessionAngle
    {
        get{
            return(x[0]);
        }

        set{
            x[0] = value;
        }
    }

    // LeanAngle, phi
    public double LeanAngle
    {
        get{
            return(x[1]);
        }

        set{
            x[1] = value;
        }
    }

    // SpinAngle, theta
    public double SpinAngle
    {
        get{
            return(x[2]);
        }

        set{
            x[2] = value;
        }
    }

    // SpinRate, omegaY
    public double SpinRate
    {
        get{
            return(x[4]);
        }

        set{
            x[4] = value;
        }
    }

    public double KineticEnergy
    {
        get{
            return ke;
        }
    }

    public double PotentialEnergy
    {
        get{
            return pe;
        }
    }

    public double AngMoY
    {
        get{
            return angMoY;
        }
    }
}