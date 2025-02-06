using UnityEngine;

// simple foot IK system by Landon
// based on : https://www.youtube.com/watch?v=rGB1ipH6DrM
// NOTES:
// Make sure IK pass is turned on in animator layer settings
// Only works on humanoid avatars
// curves for IKWeight show us as keys in .anim

// must be placed on same game object as animator for OnAnimatorIK


// workaround for GetIKPosition giving solved location instead of anim location ???
//https://github.com/Unity-Technologies/animation-jobs-samples/issues/14


[RequireComponent(typeof(Animator))] 
public class FootIK : MonoBehaviour {

	[SerializeField] LayerMask mask;
    Animator anim;

    Vector3 footOffset;
    float maxStepHeight = 0.5f;
    float maxBelowAnim = 0.2f;

    Vector3 leftPosAnim, leftPosIK;
    Vector3 rightPosAnim, rightPosIK;
    Quaternion leftRotAnim, leftRotIK;
    Quaternion rightRotAnim, rightRotIK;
    bool initialsed = false;

    void Start() {
        anim = GetComponent<Animator>();
        footOffset = Vector3.up * 0.1f;
    }

    // OnAnimatorIK() is called by the Animator Component immediately before it updates its internal IK system
    void OnAnimatorIK(int layerIndex) {
        // get the location for each foot from the animator
        // note: gives position AFTER IK solve so we have to address that later
        leftPosAnim = anim.GetIKPosition(AvatarIKGoal.LeftFoot);
        leftRotAnim = anim.GetIKRotation(AvatarIKGoal.LeftFoot);
        rightPosAnim = anim.GetIKPosition(AvatarIKGoal.RightFoot);
        rightRotAnim = anim.GetIKRotation(AvatarIKGoal.RightFoot);
        // start IKS at locations set by animator
        if (!initialsed) {
            leftPosIK = leftPosAnim;
            leftRotIK = leftRotAnim;
            rightPosIK = rightPosAnim;
            rightRotIK = rightRotAnim;
            initialsed = true;
        }
        // set IK weights from animation curves
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKWeightLeft"));
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKWeightLeft"));
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKWeightRight"));
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKWeightRight"));



        // LEFT FOOT
        // raycast to the ground 

        // ray shoots down from maxStepHeight above foot bone
        Ray ray = new Ray(leftPosAnim + Vector3.up * maxStepHeight, Vector3.down);
        RaycastHit hit;
        // do a raycast to check if ground distance is within foot range
        if (Physics.Raycast(ray, out hit, maxStepHeight + maxBelowAnim, ~mask)) {
            leftPosIK = hit.point + footOffset;
            leftRotIK = Quaternion.LookRotation(transform.forward, hit.normal);
        }
        // set the IK position to be the current position of the foot to avoid jumpyness durring weight blending
        // note: this is a work arou nd for trying to get the foot position as the animator would set it
        // because anim.GetIKPosition gives position post solve, leading to jumpyness
        else {
            leftPosIK = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            leftRotIK = leftRotAnim;
        }
        // set the IK location in the animator
        // remember that whether these are used or not still depends on weights set earlier
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftPosIK);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftRotIK);



        // RIGHT FOOT
        ray = new Ray(rightPosAnim + Vector3.up * maxStepHeight, Vector3.down);
        if (Physics.Raycast(ray, out hit, maxStepHeight + maxBelowAnim, ~mask)) {
            rightPosIK = hit.point + footOffset;
            rightRotIK = Quaternion.LookRotation(transform.forward, hit.normal);
        }
        else {
            rightPosIK = anim.GetBoneTransform(HumanBodyBones.RightFoot).position;
            rightRotIK = rightRotAnim;
        }
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightPosIK);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightRotIK);


        /*
        // Debug
        Debug.DrawRay(leftPosIK - transform.forward * 0.1f,transform.forward * 0.2f, Color.white);
        Debug.DrawRay(leftPosIK - transform.right * 0.1f,transform.right * 0.2f, Color.white);
        */
    }
}









/***** old version *************************************
    // OnAnimatorIK() is called by the Animator Component immediately before it updates its internal IK system
    void OnAnimatorIK() {

        // LEFT FOOT
        // set animator weights based on IKWeightLeft animation curve
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKWeightLeft"));
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKWeightLeft"));

        // initialise with the foot position given by the animation
        leftPos = anim.GetIKPosition(AvatarIKGoal.LeftFoot);
        leftRot = anim.GetIKRotation(AvatarIKGoal.LeftFoot);

        //Debug.DrawRay(leftPos, Vector3.up * maxStepHeight, Color.yellow);
        //Debug.DrawRay(leftPos, Vector3.down * maxBelowAnim, Color.magenta);
        // ray shoots down from maxStepHeight above foot bone
        Ray ray = new Ray(leftPos + Vector3.up * maxStepHeight, Vector3.down);
        // ray checks for collision within a distance of footOffset + maxStepHeight (max at straight leg)
        float rayDistance = maxStepHeight + maxBelowAnim;
        // update left foot position and rotation if raycast hits something, ignoring masked layers
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, footOffset + maxStepHeight, ~mask)) {
            leftPos = hit.point + Vector3.up * footOffset;
            leftRot = Quaternion.LookRotation(transform.forward, hit.normal);
        }

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftPos);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftRot);


        // RIGHT FOOT
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKWeightRight"));
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKWeightRight"));
        rightPos = anim.GetIKPosition(AvatarIKGoal.RightFoot);
        rightRot = anim.GetIKRotation(AvatarIKGoal.RightFoot);
        ray = new Ray(rightPos + Vector3.up * maxStepHeight, Vector3.down);
        rayDistance = maxStepHeight + maxBelowAnim;
        if (Physics.Raycast(ray, out hit, footOffset + maxStepHeight, ~mask)) {
            rightPos = hit.point + Vector3.up * footOffset;
            rightRot = Quaternion.LookRotation(transform.forward, hit.normal);
        }
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightPos);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightRot);
    }
}************************************************************************************************/


/**** Idle Only Version (doesn't require animation curves)*************
    // check if idle
    idle = (ic.move.magnitude < 0.1f && ic.grounded);
    // if not idle, IK weights are 0. No need to calculate IK positions
    if (!idle) {
        weightIK = 0f;
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weightIK);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, weightIK);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, weightIK);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, weightIK);
        return;
    }
    // smoothly blend towards using IKS while idle
    weightIK = Mathf.Lerp(weightIK, 1f, 3f * Time.deltaTime);
    // .... the rest as done in curve version ....
**********************************************************************/