using UnityEngine;

[RequireComponent(typeof(MoveAnimalInput))]
public class MoveDogInput : MoveAnimalInput {

    public override void GatherInput() {
        Transform m_Destination = m_Animal.getDestination();
        if (m_Destination == null) {
            m_Axis = Vector2.zero;
            return;
        }
        Vector3 direction = m_Destination.position - transform.position;
        direction.y = 0f;
        if (direction.magnitude <= 1.5f) {
            m_Axis = Vector2.zero;
            return;
        }

        m_IsJump = false;
        m_IsRun = direction.magnitude>5f;

        direction.Normalize();
        m_Axis = new Vector2(direction.x, direction.z);

        m_Target = m_Destination.position;
    }
}