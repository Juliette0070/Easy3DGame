using UnityEngine;

[RequireComponent(typeof(ithappy.Animals_FREE.CreatureMover))]
public class MoveAnimalInput : MonoBehaviour {

    protected ithappy.Animals_FREE.CreatureMover m_Mover;

    protected Vector2 m_Axis;
    protected bool m_IsRun;
    protected bool m_IsJump;

    protected Vector3 m_Target;

    protected IAnimal m_Animal;

    private void Awake() {
        m_Mover = GetComponent<ithappy.Animals_FREE.CreatureMover>();
        m_Animal = GetComponent<IAnimal>();
    }

    private void Update() {
        GatherInput();
        SetInput();
    }

    public virtual void GatherInput() {
        m_IsRun = false;
        m_IsJump = false;

        Transform m_Destination = m_Animal.getDestination();
        if (m_Destination == null) {
            m_Axis = Vector2.zero;
            return;
        }
        Vector3 direction = m_Destination.position - transform.position;
        direction.y = 0f;
        if (direction.magnitude <= 0.2f) {
            m_Axis = Vector2.zero;
            return;
        }
        direction.Normalize();
        m_Axis = new Vector2(direction.x, direction.z);

        m_Target = m_Destination.position;
        
        // Debug.DrawRay(transform.position, direction * 2f, Color.red);
        // Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
    }

    public void SetInput() {
        m_Mover?.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);
    }
}