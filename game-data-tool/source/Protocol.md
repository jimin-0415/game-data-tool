# Protocol

### Item
- PacketBegin : 100

[>>] PktItemGet 
// 아이템 획득 요청 
	- int m_id;      // 회득한 아이템 아이디


[<<] PktItemGetResult
// 아이템 획득 결과
	- int m_id;      // 회득한 아이템 아이디

[<<] PktItemGetNotify
// 아이템 획득 알림
	- int m_id;      // 회득한 아이템 아이디

---

### Skill
- PacketBegin : 200

[>>] PktSkillStart
// 스킬 시작
	- int m_skillId; // 시작 스킬 아이디