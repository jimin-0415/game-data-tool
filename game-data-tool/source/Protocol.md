# Protocol

### Item
- PacketBegin : 100

[>>] PktItemGet 
// ������ ȹ�� ��û 
	- int m_id;      // ȸ���� ������ ���̵�


[<<] PktItemGetResult
// ������ ȹ�� ���
	- int m_id;      // ȸ���� ������ ���̵�

[<<] PktItemGetNotify
// ������ ȹ�� �˸�
	- int m_id;      // ȸ���� ������ ���̵�

---

### Skill
- PacketBegin : 200

[>>] PktSkillStart
// ��ų ����
	- int m_skillId; // ���� ��ų ���̵�