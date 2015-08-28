using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	//状态数组，用来检测不同状态切换不同碰撞体
	public string[] states = {"Idle", "Move", "Battle_move", "IdleStandby", "Standby", "Shift", "Atk_1", "Atk_2", "Atk_3", "Atk_4", "Jump_up", "Jump_falling", "Jump_over"};
	/*----------------------------------------------------------------------------*/
	public float direction = 1;
	public float move_spped = 5;
	public float jump_speed = 10;
	public float sprint_speed = 10;//shift速度
	private ArrayList attackActions;//普通攻击序列
	private ArrayList skillActions;//技能攻击序列
	private int normalAttack_index = 0;
	private int skill_index = 0;
	private bool attack_combo = false;
	private bool toAttack = true;//是否首次进入攻击状态
	private Animator m_animator;
	private AnimatorStateInfo current_stateInfo;
	private string current_state = "Idle";
	
	// Use this for initialization
	void Start () {
		
	}
	
	void Awake() {
		m_animator = GetComponent<Animator>();
		attackActions = new ArrayList();
		skillActions = new ArrayList();
		int[] temp = new int[]{1,2};
		for(int i=0;i<temp.Length;i++) {
			attackActions.Add(temp[i]);
		}
		//初始化技能序列
		int[] temp1 = new int[]{1,2,3,4,5,6};
		for(int i=0;i<temp1.Length;i++) {
			skillActions.Add(temp1[i]);
		}	
	}
	
	// Update is called once per frame
	void Update () {
		
		float moveRate,horizontal = Input.GetAxis("Horizontal");
		 current_stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
		if(current_stateInfo.IsTag("NormalAttack") || current_stateInfo.IsTag("SkillAttack")) {
			if(toAttack) {
				m_animator.SetInteger("attackAction", 0);
				m_animator.SetInteger("skillAction", 0);
				toAttack = false;
			}
			if(!attack_combo) {
				if(Input.GetButtonDown("Fire2")) {
					normalAttack_index = 0;
					m_animator.SetInteger("attackAction", 0);
					m_animator.SetInteger("skillAction",(int)skillActions[skill_index]);
					skill_index = (skill_index+1)%skillActions.Count;
					attack_combo = true;
				}
				else if(Input.GetButtonDown("Fire1")) {
					m_animator.SetInteger("skillAction", 0);
					m_animator.SetInteger("attackAction",(int)attackActions[normalAttack_index]);
					normalAttack_index = (normalAttack_index+1)%attackActions.Count;
					attack_combo = true;
				}
			}
		}
		else if(current_stateInfo.IsName("Shift")) {
			if(current_stateInfo.normalizedTime>0.9f) {
				m_animator.SetBool("shift", false);
			}
			transform.Translate(direction*sprint_speed*Time.deltaTime*Vector3.right);
		}
		else if(current_stateInfo.IsName("IdleStandby")){}
		else {
			toAttack = true;
			attack_combo = false;
			if(current_stateInfo.IsName("Idle")) {
			m_animator.SetBool("normalState", true);
			}else if(current_stateInfo.IsTag("NextToAttack")) {
				if(Input.GetButtonDown("Fire2")) {
					m_animator.SetInteger("skillAction", (int)skillActions[skill_index]);
					skill_index = (skill_index+1)%skillActions.Count;
				}
				else if(Input.GetButtonDown("Fire1")) {
					normalAttack_index = 0;
					m_animator.SetInteger("attackAction", (int)attackActions[normalAttack_index]);
					normalAttack_index = (normalAttack_index+1)%attackActions.Count;
				}
				else if(Input.GetButtonDown("Jump")) {
					m_animator.SetBool("shift", true);
				}
			}
			//暂时设置更改正常至战斗状态的按键为T
			if(Input.GetKeyDown(KeyCode.T)) {
				m_animator.SetBool("normalState", !m_animator.GetBool("normalState"));
			}
			//  if(Input.GetButtonDown("Jump")) {
			//  	m_animator.SetBool("jumping", true);
			//  	
			//  }
			moveRate = Mathf.Abs(horizontal);
			m_animator.SetFloat("moveRate", moveRate);
			if(horizontal > 0) {
				direction = 1;
				transform.localScale = new Vector3(direction, 1, 1);
			}else if(horizontal < 0) {
				direction = -1;
				transform.localScale = new Vector3(direction, 1, 1);
			}
			transform.Translate(direction*moveRate*move_spped*Time.deltaTime*Vector3.right);
			//transform.Translate(Time.deltaTime*jump_speed*Vector3.up);
		}
	}
	
	void LateUpdate() {
		//碰撞体的切换,此处注释是因为有些碰撞体仍需修改或添加
		//  if(!current_stateInfo.IsName(current_state))
		//  for(int i=0; i<states.Length; i++) {
		//  	if(current_stateInfo.IsName(states[i]))
		//  		setCollider(states[i]);
		//  }
	}
	
	void FixedUpdate() {
		
	}
	//设置当前碰撞体
	void setCollider(string curState) {
		GameObject.Find("KarateGirl/model/Collider/"+current_state).SetActive(false);
		current_state = curState;
		GameObject.Find("KarateGirl/model/Collider/"+curState).SetActive(true);
	}
	//设置技能队列
	public void setComboQueue(int[] queue) {
		skillActions.Clear();
		for(int i=0;i<queue.Length;i++) {
			skillActions.Add(queue[i]);
		}
	}
	
}
