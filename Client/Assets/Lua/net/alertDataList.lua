------------------- AlertDataList ---------------
AlertDataList= { 
	DataList= { 

		-- 成功 
		ok = 'LC_ALERT_ok',

		-- 内部错误 
		inner_error = 'LC_ALERT_inner_error',

		-- 参数错误 
		bad_param = 'LC_ALERT_bad_param',

		-- 服务器超时 
		timeout = 'LC_ALERT_timeout',

		-- 用户名已被注册 
		usr_name_duplicate = 'LC_ALERT_usr_name_duplicate',

		-- 用户名/UDID校验错误 
		usr_udid_failed = 'LC_ALERT_usr_udid_failed',

		-- 注册失败 
		usr_register_error = 'LC_ALERT_usr_register_error',

		-- 用户被封号 
		usr_banned = 'LC_ALERT_usr_banned',

		-- 用户被禁言 
		usr_muted = 'LC_ALERT_usr_muted',

		-- 不存在的卡牌 
		card_inexistence = 'LC_ALERT_card_inexistence',

		-- 没有此id卡牌 
		card_delte_inexistence = 'LC_ALERT_card_delte_inexistence',

		-- 不存在的卡牌 
		group_inexistence = 'LC_ALERT_group_inexistence',

		-- 卡牌数量超过最大值 
		group_max_card = 'LC_ALERT_group_max_card',

		-- 不存在的符文 
		rune_inexistence = 'LC_ALERT_rune_inexistence',

		-- 不存在的符文 
		rune_delte_inexistence = 'LC_ALERT_rune_delte_inexistence',

		-- 符文数量超过最大值 
		group_max_rune = 'LC_ALERT_group_max_rune',

		-- 不存在的剧情id 
		story_inexistence = 'LC_ALERT_story_inexistence',

		-- 没有通过前置剧情 
		story_parentid_no_pass = 'LC_ALERT_story_parentid_no_pass',

		-- 挑战的信息不全 
		story_enemyinfo_error = 'LC_ALERT_story_enemyinfo_error',

		-- 没有在战斗中 
		fight_state_no_battle = 'LC_ALERT_fight_state_no_battle',

		-- 不允许切换状态 
		fight_state_challge_error = 'LC_ALERT_fight_state_challge_error',

		-- 不允许切换状态 
		fight_state_ai_error = 'LC_ALERT_fight_state_ai_error',

		-- 不是cd冷却结束的卡牌 
		fight_state_cdend_data_error = 'LC_ALERT_fight_state_cdend_data_error',

		-- 只能开启一场战斗 
		fight_state_in_battle = 'LC_ALERT_fight_state_in_battle',

		-- 不知道要挑战的对象 
		fight_champion_noenemy = 'LC_ALERT_fight_champion_noenemy',

		-- 你不能挑战此排名 
		fight_champion_area = 'LC_ALERT_fight_champion_area',

		-- 要挑战的玩家正在忙 
		fight_champion_rank_lock = 'LC_ALERT_fight_champion_rank_lock',

		-- 没有玩家 
		fight_freedom_playerlist_null = 'LC_ALERT_fight_freedom_playerlist_null',

		-- 不公平等级不同 
		fight_freedom_level_unfair = 'LC_ALERT_fight_freedom_level_unfair',

		-- 不是好友关系 
		fight_friend_unfriend = 'LC_ALERT_fight_friend_unfriend',

		-- 已经超过允许获得卡牌最大数 
		card_full = 'LC_ALERT_card_full',

		-- 在卡组中的卡牌不支持此操作 
		card_delete_ingroup = 'LC_ALERT_card_delete_ingroup',

		-- 已经超过允许获得卡组最大数 
		group_full = 'LC_ALERT_group_full',

		-- 已经超过允许等级可以加入卡牌最大数 
		group_max_card_level = 'LC_ALERT_group_max_card_level',

		-- 已经超过允许等级可以加入卡牌最大COST 
		group_max_cost_level = 'LC_ALERT_group_max_cost_level',

		-- 已经超过允许获得符文最大数 
		rune_full = 'LC_ALERT_rune_full',

		-- 在卡组中的符文不支持此操作 
		rune_delete_ingroup = 'LC_ALERT_rune_delete_ingroup',

		-- 已经超过允许等级可以加入符文最大数 
		group_max_rune_level = 'LC_ALERT_group_max_rune_level',

		-- 已经超过允许卡牌强化最大数 
		enhance_card_max_level = 'LC_ALERT_enhance_card_max_level',

		-- 已经超过允许符文强化最大数 
		enhance_rune_max_level = 'LC_ALERT_enhance_rune_max_level',

		-- 没有通剧情不能探索 
		story_no_pass = 'LC_ALERT_story_no_pass',

		-- 不存在的迷宫id 
		maze_inexistence = 'LC_ALERT_maze_inexistence',

		-- 迷宫已经通关请重置 
		maze_passed = 'LC_ALERT_maze_passed',

		-- 迷宫配置的怪物错误 
		maze_enemy_inexistence = 'LC_ALERT_maze_enemy_inexistence',

		-- 挑战的迷宫信息不全 
		maze_enemyinfo_error = 'LC_ALERT_maze_enemyinfo_error',

		-- 每日重置迷宫达到上限 
		maze_reset_max = 'LC_ALERT_maze_reset_max',

		-- 不能装配的符文 
		rune_unequip = 'LC_ALERT_rune_unequip',

		-- 对手战斗信息不全 
		fight_enemyinfo_error = 'LC_ALERT_fight_enemyinfo_error',

		-- 开启自由切磋等级不足 
		fight_freedom_min_level = 'LC_ALERT_fight_freedom_min_level',

		-- 玩家不存在 
		user_not_exist = 'LC_ALERT_user_not_exist',

		-- 金钱满了 
		prize_money_max = 'LC_ALERT_prize_money_max',

		-- 行动力满了 
		prize_power_max = 'LC_ALERT_prize_power_max',

		-- 行动力不足 
		cost_power_less = 'LC_ALERT_cost_power_less',

		-- 金钱不足 
		cost_money_less = 'LC_ALERT_cost_money_less',

		-- 晶钻不足 
		cost_gold_less = 'LC_ALERT_cost_gold_less',

		-- 宝箱ID错误 
		chest_error_id = 'LC_ALERT_chest_error_id',

		-- 晶钻满了 
		prize_gold_max = 'LC_ALERT_prize_gold_max',

		-- 魔法卡满了 
		prize_magic_max = 'LC_ALERT_prize_magic_max',

		-- 魔法卡不足 
		cost_magic_max = 'LC_ALERT_cost_magic_max',

		-- 魔法卷类型错误 
		magic_scroll_type = 'LC_ALERT_magic_scroll_type',

		-- 魔法卷满了 
		magic_scroll_max = 'LC_ALERT_magic_scroll_max',

		-- 卡牌等级过大 
		card_more_lev = 'LC_ALERT_card_more_lev',

		-- 已经达到最大挑战次数 
		champion_max_times = 'LC_ALERT_champion_max_times',

		-- 冷却中不能挑战 
		champion_cooldown = 'LC_ALERT_champion_cooldown',

		-- 冷却时间已经结束 
		champion_clean_cdend = 'LC_ALERT_champion_clean_cdend',

		-- 魔法卷不足 
		magic_scroll_less = 'LC_ALERT_magic_scroll_less',

		-- 自由切磋冷却中不能挑战 
		freedom_cooldown = 'LC_ALERT_freedom_cooldown',

		-- 自由切磋冷却时间已经结束 
		freedom_clean_cdend = 'LC_ALERT_freedom_clean_cdend',

		-- 已经有盗贼不能再创建 
		monster_creat_exist = 'LC_ALERT_monster_creat_exist',

		-- 配置中没有此等级盗贼 
		monster_gd_inexistence = 'LC_ALERT_monster_gd_inexistence',

		-- 挑战盗贼冷却中 
		monster_cooldown = 'LC_ALERT_monster_cooldown',

		-- 盗贼正在被攻击 
		monster_fight_lock = 'LC_ALERT_monster_fight_lock',

		-- 已经被杀死 
		monster_fight_end = 'LC_ALERT_monster_fight_end',

		-- 没有地图产出 
		map_reward_zero = 'LC_ALERT_map_reward_zero',

		-- 挑战盗贼冷却时间已经结束 
		monster_clean_cdend = 'LC_ALERT_monster_clean_cdend',

		-- 战斗LOG过期 
		fight_log_expired = 'LC_ALERT_fight_log_expired',

		-- 挑战魔神冷却中 
		crazy_cooldown = 'LC_ALERT_crazy_cooldown',

		-- 魔神正在被攻击 
		crazy_fight_lock = 'LC_ALERT_crazy_fight_lock',

		-- 魔神不存在 
		crazy_fight_end = 'LC_ALERT_crazy_fight_end',

		-- 挑战的盗贼信息不全 
		monster_enemyinfo_error = 'LC_ALERT_monster_enemyinfo_error',

		-- 挑战的魔神信息不全 
		crazy_enemyinfo_error = 'LC_ALERT_crazy_enemyinfo_error',

		-- 挑战魔神冷却时间已经结束 
		crazy_clean_cdend = 'LC_ALERT_crazy_clean_cdend',

		-- 换头像失败，没有对应卡牌 
		change_icon_fail = 'LC_ALERT_change_icon_fail',

		-- 账号错误，不能设置为facebook头像 
		change_icon_accounttype = 'LC_ALERT_change_icon_accounttype',

		-- 名字相同，不能更换 
		change_name_same = 'LC_ALERT_change_name_same',

		-- 名字太短 
		username_too_short = 'LC_ALERT_username_too_short',

		-- 名字太长 
		username_too_long = 'LC_ALERT_username_too_long',

		-- 名字含有非法字符 
		username_unvalid = 'LC_ALERT_username_unvalid',

		-- 私聊对象不存在 
		chat_receiver_not = 'LC_ALERT_chat_receiver_not',

		-- 被禁言 
		chat_mute = 'LC_ALERT_chat_mute',

		-- 改名不能以player开头 
		change_name_player = 'LC_ALERT_change_name_player',

		-- 已经完成了这个新手引导 
		already_finsh = 'LC_ALERT_already_finsh',

		-- 参战卡组中没有配置卡牌 
		group_empty_card = 'LC_ALERT_group_empty_card',

		-- 不能添加自己 
		friend_add_self = 'LC_ALERT_friend_add_self',

		-- 自己好友已满 
		friend_max = 'LC_ALERT_friend_max',

		-- 对方好友已满 
		friend_target_max = 'LC_ALERT_friend_target_max',

		-- 已经是好友 
		friend_had_add = 'LC_ALERT_friend_had_add',

		-- 删除已满 
		friend_del_max = 'LC_ALERT_friend_del_max',

		-- 不是好友 
		friend_not_friend = 'LC_ALERT_friend_not_friend',

		-- 没有申请，不能加好友 
		friend_no_apply = 'LC_ALERT_friend_no_apply',

		-- 对方没有求救 
		gift_no_help = 'LC_ALERT_gift_no_help',

		-- 行动力不足不能赠送 
		gift_no_power = 'LC_ALERT_gift_no_power',

		-- 对方没有赠送 
		gift_no_give = 'LC_ALERT_gift_no_give',

		-- 行动力太多，不能领 
		gift_power_max = 'LC_ALERT_gift_power_max',

		-- 领取次数太多 
		gift_get_max = 'LC_ALERT_gift_get_max',

		-- 已经求救过 
		gift_has_help = 'LC_ALERT_gift_has_help',

		-- 不能操作自己 
		gift_not_self = 'LC_ALERT_gift_not_self',

		-- 已领取该成就物品 
		grade_has_gain = 'LC_ALERT_grade_has_gain',

		-- 没完成该成就 
		grade_not_finish = 'LC_ALERT_grade_not_finish',

		-- 没有足够的金钱了 
		not_enough_money = 'LC_ALERT_not_enough_money',

		-- 冥想槽一满了，请处理 
		site_full = 'LC_ALERT_site_full',

		-- 碎片不足，无法兑换 
		fragment_not_enough = 'LC_ALERT_fragment_not_enough',

		-- 没有可处理的碎片 
		not_has_fragment = 'LC_ALERT_not_has_fragment',

		-- 神庙掉落配置错误 
		temple_drop_cfg = 'LC_ALERT_temple_drop_cfg',

		-- 神庙兑换配置错误 
		temple_exc_cfg = 'LC_ALERT_temple_exc_cfg',

		-- 已经有军团了 
		legion_has_join = 'LC_ALERT_legion_has_join',

		-- 没有军团 
		legion_not_join = 'LC_ALERT_legion_not_join',

		-- 目标已有军团 
		legion_taget_has = 'LC_ALERT_legion_taget_has',

		-- 创建军团等级不足 
		legion_create_lev = 'LC_ALERT_legion_create_lev',

		-- 创建晶军团钻不足 
		legion_create_cost = 'LC_ALERT_legion_create_cost',

		-- 军团已满 
		legion_meb_max = 'LC_ALERT_legion_meb_max',

		-- 军团未找到 
		legion_not_find = 'LC_ALERT_legion_not_find',

		-- 军团无此人 
		legion_no_meb = 'LC_ALERT_legion_no_meb',

		-- 加入军团等级不足 
		legion_join_lev = 'LC_ALERT_legion_join_lev',

		-- 已经申请过了 
		legion_has_apply = 'LC_ALERT_legion_has_apply',

		-- 没有入团申请 
		legion_no_apply = 'LC_ALERT_legion_no_apply',

		-- 已经邀请过了 
		legion_has_invite = 'LC_ALERT_legion_has_invite',

		-- 没有入团邀请 
		legion_no_invite = 'LC_ALERT_legion_no_invite',

		-- 军团权限不足 
		legion_not_right = 'LC_ALERT_legion_not_right',

		-- 每日捐钱上限 
		legion_money_max = 'LC_ALERT_legion_money_max',

		-- 每日捐晶钻上限 
		legion_gold_max = 'LC_ALERT_legion_gold_max',

		-- 军团配置错误 
		legion_config_error = 'LC_ALERT_legion_config_error',

		-- 军团等级已满 
		legion_lev_max = 'LC_ALERT_legion_lev_max',

		-- 军团购买商品等级不足 
		legion_goods_lev = 'LC_ALERT_legion_goods_lev',

		-- 军团ID错误 
		legion_id_error = 'LC_ALERT_legion_id_error',

		-- 军团积分战CD限制 
		legion_challenge_cd_limit = 'LC_ALERT_legion_challenge_cd_limit',

		-- 挑战对象错误。 
		legion_challenge_bad_id = 'LC_ALERT_legion_challenge_bad_id',

		-- 捐献金币不足 
		legion_devote_money = 'LC_ALERT_legion_devote_money',

		-- 捐献晶钻不足 
		legion_devote_gold = 'LC_ALERT_legion_devote_gold',

		-- 捐献过少 
		legion_devote_less = 'LC_ALERT_legion_devote_less',

		-- 军团积分战挑战次数限制 
		legion_challenge_cnt_limit = 'LC_ALERT_legion_challenge_cnt_limit',

		-- 军团名字重复 
		legion_name_redo = 'LC_ALERT_legion_name_redo',

		-- 不能挑战同一军团的成员 
		legion_challenge_same = 'LC_ALERT_legion_challenge_same',

		-- 超过每日删除人数上限 
		legion_del_more = 'LC_ALERT_legion_del_more',

		-- 没有这条配置 
		market_no_config = 'LC_ALERT_market_no_config',

		-- 作弊了吧，没理由看到这一条 
		market_no_reason = 'LC_ALERT_market_no_reason',

		-- 不够扣 
		market_no_cost = 'LC_ALERT_market_no_cost',

		-- 商城配置错误 
		market_config_error = 'LC_ALERT_market_config_error',

		-- 签到天数不够 
		sign_not_this = 'LC_ALERT_sign_not_this',

		-- 签到已经领取 
		sign_has_gain = 'LC_ALERT_sign_has_gain',

		-- 攻击次数不足 
		gvg_attack_less = 'LC_ALERT_gvg_attack_less',

		-- 没有这个玩家 
		gvg_no_player = 'LC_ALERT_gvg_no_player',

		-- 攻击类型错误 
		gvg_fight_type_error = 'LC_ALERT_gvg_fight_type_error',

		-- 自己军团不在战场，数据错误 
		gvg_not_battle = 'LC_ALERT_gvg_not_battle',

		-- 对方军团不在战场，数据错误 
		gvg_target_not_battle = 'LC_ALERT_gvg_target_not_battle',

		-- 攻击目标错误 
		gvg_target_error = 'LC_ALERT_gvg_target_error',

		-- 护盾已被击破 
		gvg_shield_destroy = 'LC_ALERT_gvg_shield_destroy',

		-- 护盾未击破 
		gvg_shield_not_destroy = 'LC_ALERT_gvg_shield_not_destroy',

		-- 匹配失败 
		gvg_match_fail = 'LC_ALERT_gvg_match_fail',

		-- GVG战场已经开始 
		gvg_battle_begin = 'LC_ALERT_gvg_battle_begin',

		-- GVG战场已经加入 
		gvg_battle_has_jion = 'LC_ALERT_gvg_battle_has_jion',

		-- GVG没有开始 
		gvg_not_start = 'LC_ALERT_gvg_not_start',

		-- GVG战场时间不足 
		gvg_no_time = 'LC_ALERT_gvg_no_time',

		-- GVG攻击次数已满 
		gvg_attack_max = 'LC_ALERT_gvg_attack_max',

		-- GVG购买护盾类型不对 
		gvg_buy_shield_type = 'LC_ALERT_gvg_buy_shield_type',

		-- GVG军团信息不存在 
		gvg_not_legion = 'LC_ALERT_gvg_not_legion',
	},
} 
AlertDataList.CodeToKey = { }
AlertDataList.CodeToKey[10000] = {
	kind = 'Tip',
	text = 'ok',
}
AlertDataList.CodeToKey[10001] = {
	kind = 'Tip',
	text = 'inner_error',
}
AlertDataList.CodeToKey[10002] = {
	kind = 'Tip',
	text = 'bad_param',
}
AlertDataList.CodeToKey[10003] = {
	kind = 'Tip',
	text = 'timeout',
}
AlertDataList.CodeToKey[1] = {
	kind = 'Tip',
	text = 'usr_name_duplicate',
}
AlertDataList.CodeToKey[2] = {
	kind = 'Tip',
	text = 'usr_udid_failed',
}
AlertDataList.CodeToKey[3] = {
	kind = 'Tip',
	text = 'usr_register_error',
}
AlertDataList.CodeToKey[4] = {
	kind = 'Tip',
	text = 'usr_banned',
}
AlertDataList.CodeToKey[5] = {
	kind = 'Tip',
	text = 'usr_muted',
}
AlertDataList.CodeToKey[100] = {
	kind = 'Tip',
	text = 'card_inexistence',
}
AlertDataList.CodeToKey[101] = {
	kind = 'Tip',
	text = 'card_delte_inexistence',
}
AlertDataList.CodeToKey[102] = {
	kind = 'Tip',
	text = 'group_inexistence',
}
AlertDataList.CodeToKey[103] = {
	kind = 'Tip',
	text = 'group_max_card',
}
AlertDataList.CodeToKey[104] = {
	kind = 'Tip',
	text = 'rune_inexistence',
}
AlertDataList.CodeToKey[105] = {
	kind = 'Tip',
	text = 'rune_delte_inexistence',
}
AlertDataList.CodeToKey[106] = {
	kind = 'Tip',
	text = 'group_max_rune',
}
AlertDataList.CodeToKey[107] = {
	kind = 'Tip',
	text = 'story_inexistence',
}
AlertDataList.CodeToKey[108] = {
	kind = 'Tip',
	text = 'story_parentid_no_pass',
}
AlertDataList.CodeToKey[109] = {
	kind = 'Tip',
	text = 'story_enemyinfo_error',
}
AlertDataList.CodeToKey[110] = {
	kind = 'Tip',
	text = 'fight_state_no_battle',
}
AlertDataList.CodeToKey[111] = {
	kind = 'Tip',
	text = 'fight_state_challge_error',
}
AlertDataList.CodeToKey[112] = {
	kind = 'Tip',
	text = 'fight_state_ai_error',
}
AlertDataList.CodeToKey[113] = {
	kind = 'Tip',
	text = 'fight_state_cdend_data_error',
}
AlertDataList.CodeToKey[114] = {
	kind = 'Tip',
	text = 'fight_state_in_battle',
}
AlertDataList.CodeToKey[115] = {
	kind = 'Tip',
	text = 'fight_champion_noenemy',
}
AlertDataList.CodeToKey[116] = {
	kind = 'Tip',
	text = 'fight_champion_area',
}
AlertDataList.CodeToKey[117] = {
	kind = 'Tip',
	text = 'fight_champion_rank_lock',
}
AlertDataList.CodeToKey[118] = {
	kind = 'Tip',
	text = 'fight_freedom_playerlist_null',
}
AlertDataList.CodeToKey[119] = {
	kind = 'Tip',
	text = 'fight_freedom_level_unfair',
}
AlertDataList.CodeToKey[120] = {
	kind = 'Tip',
	text = 'fight_friend_unfriend',
}
AlertDataList.CodeToKey[121] = {
	kind = 'Tip',
	text = 'card_full',
}
AlertDataList.CodeToKey[122] = {
	kind = 'Tip',
	text = 'card_delete_ingroup',
}
AlertDataList.CodeToKey[123] = {
	kind = 'Tip',
	text = 'group_full',
}
AlertDataList.CodeToKey[124] = {
	kind = 'Tip',
	text = 'group_max_card_level',
}
AlertDataList.CodeToKey[125] = {
	kind = 'Tip',
	text = 'group_max_cost_level',
}
AlertDataList.CodeToKey[126] = {
	kind = 'Tip',
	text = 'rune_full',
}
AlertDataList.CodeToKey[127] = {
	kind = 'Tip',
	text = 'rune_delete_ingroup',
}
AlertDataList.CodeToKey[128] = {
	kind = 'Tip',
	text = 'group_max_rune_level',
}
AlertDataList.CodeToKey[129] = {
	kind = 'Tip',
	text = 'enhance_card_max_level',
}
AlertDataList.CodeToKey[130] = {
	kind = 'Tip',
	text = 'enhance_rune_max_level',
}
AlertDataList.CodeToKey[131] = {
	kind = 'Tip',
	text = 'story_no_pass',
}
AlertDataList.CodeToKey[132] = {
	kind = 'Tip',
	text = 'maze_inexistence',
}
AlertDataList.CodeToKey[133] = {
	kind = 'Tip',
	text = 'maze_passed',
}
AlertDataList.CodeToKey[134] = {
	kind = 'Tip',
	text = 'maze_enemy_inexistence',
}
AlertDataList.CodeToKey[135] = {
	kind = 'Tip',
	text = 'maze_enemyinfo_error',
}
AlertDataList.CodeToKey[136] = {
	kind = 'Tip',
	text = 'maze_reset_max',
}
AlertDataList.CodeToKey[137] = {
	kind = 'Tip',
	text = 'rune_unequip',
}
AlertDataList.CodeToKey[138] = {
	kind = 'Tip',
	text = 'fight_enemyinfo_error',
}
AlertDataList.CodeToKey[139] = {
	kind = 'Tip',
	text = 'fight_freedom_min_level',
}
AlertDataList.CodeToKey[140] = {
	kind = 'Tip',
	text = 'user_not_exist',
}
AlertDataList.CodeToKey[150] = {
	kind = 'Tip',
	text = 'prize_money_max',
}
AlertDataList.CodeToKey[151] = {
	kind = 'Tip',
	text = 'prize_power_max',
}
AlertDataList.CodeToKey[152] = {
	kind = 'Tip',
	text = 'cost_power_less',
}
AlertDataList.CodeToKey[153] = {
	kind = 'Tip',
	text = 'cost_money_less',
}
AlertDataList.CodeToKey[154] = {
	kind = 'Tip',
	text = 'cost_gold_less',
}
AlertDataList.CodeToKey[155] = {
	kind = 'Tip',
	text = 'chest_error_id',
}
AlertDataList.CodeToKey[156] = {
	kind = 'Tip',
	text = 'prize_gold_max',
}
AlertDataList.CodeToKey[157] = {
	kind = 'Tip',
	text = 'prize_magic_max',
}
AlertDataList.CodeToKey[158] = {
	kind = 'Tip',
	text = 'cost_magic_max',
}
AlertDataList.CodeToKey[159] = {
	kind = 'Tip',
	text = 'magic_scroll_type',
}
AlertDataList.CodeToKey[160] = {
	kind = 'Tip',
	text = 'magic_scroll_max',
}
AlertDataList.CodeToKey[161] = {
	kind = 'Tip',
	text = 'card_more_lev',
}
AlertDataList.CodeToKey[162] = {
	kind = 'Tip',
	text = 'champion_max_times',
}
AlertDataList.CodeToKey[163] = {
	kind = 'Tip',
	text = 'champion_cooldown',
}
AlertDataList.CodeToKey[164] = {
	kind = 'Tip',
	text = 'champion_clean_cdend',
}
AlertDataList.CodeToKey[165] = {
	kind = 'Tip',
	text = 'magic_scroll_less',
}
AlertDataList.CodeToKey[166] = {
	kind = 'Tip',
	text = 'freedom_cooldown',
}
AlertDataList.CodeToKey[167] = {
	kind = 'Tip',
	text = 'freedom_clean_cdend',
}
AlertDataList.CodeToKey[168] = {
	kind = 'Tip',
	text = 'monster_creat_exist',
}
AlertDataList.CodeToKey[169] = {
	kind = 'Tip',
	text = 'monster_gd_inexistence',
}
AlertDataList.CodeToKey[170] = {
	kind = 'Tip',
	text = 'monster_cooldown',
}
AlertDataList.CodeToKey[171] = {
	kind = 'Tip',
	text = 'monster_fight_lock',
}
AlertDataList.CodeToKey[172] = {
	kind = 'Tip',
	text = 'monster_fight_end',
}
AlertDataList.CodeToKey[173] = {
	kind = 'Tip',
	text = 'map_reward_zero',
}
AlertDataList.CodeToKey[174] = {
	kind = 'Tip',
	text = 'monster_clean_cdend',
}
AlertDataList.CodeToKey[175] = {
	kind = 'Tip',
	text = 'fight_log_expired',
}
AlertDataList.CodeToKey[176] = {
	kind = 'Tip',
	text = 'crazy_cooldown',
}
AlertDataList.CodeToKey[177] = {
	kind = 'Tip',
	text = 'crazy_fight_lock',
}
AlertDataList.CodeToKey[178] = {
	kind = 'Tip',
	text = 'crazy_fight_end',
}
AlertDataList.CodeToKey[179] = {
	kind = 'Tip',
	text = 'monster_enemyinfo_error',
}
AlertDataList.CodeToKey[180] = {
	kind = 'Tip',
	text = 'crazy_enemyinfo_error',
}
AlertDataList.CodeToKey[181] = {
	kind = 'Tip',
	text = 'crazy_clean_cdend',
}
AlertDataList.CodeToKey[182] = {
	kind = 'Tip',
	text = 'change_icon_fail',
}
AlertDataList.CodeToKey[183] = {
	kind = 'Tip',
	text = 'change_icon_accounttype',
}
AlertDataList.CodeToKey[184] = {
	kind = 'Tip',
	text = 'change_name_same',
}
AlertDataList.CodeToKey[185] = {
	kind = 'Tip',
	text = 'username_too_short',
}
AlertDataList.CodeToKey[186] = {
	kind = 'Tip',
	text = 'username_too_long',
}
AlertDataList.CodeToKey[187] = {
	kind = 'Tip',
	text = 'username_unvalid',
}
AlertDataList.CodeToKey[188] = {
	kind = 'Tip',
	text = 'chat_receiver_not',
}
AlertDataList.CodeToKey[189] = {
	kind = 'Tip',
	text = 'chat_mute',
}
AlertDataList.CodeToKey[190] = {
	kind = 'Tip',
	text = 'change_name_player',
}
AlertDataList.CodeToKey[191] = {
	kind = 'Tip',
	text = 'already_finsh',
}
AlertDataList.CodeToKey[192] = {
	kind = 'Tip',
	text = 'group_empty_card',
}
AlertDataList.CodeToKey[300] = {
	kind = 'Tip',
	text = 'friend_add_self',
}
AlertDataList.CodeToKey[301] = {
	kind = 'Tip',
	text = 'friend_max',
}
AlertDataList.CodeToKey[302] = {
	kind = 'Tip',
	text = 'friend_target_max',
}
AlertDataList.CodeToKey[303] = {
	kind = 'Tip',
	text = 'friend_had_add',
}
AlertDataList.CodeToKey[305] = {
	kind = 'Tip',
	text = 'friend_del_max',
}
AlertDataList.CodeToKey[306] = {
	kind = 'Tip',
	text = 'friend_not_friend',
}
AlertDataList.CodeToKey[307] = {
	kind = 'Tip',
	text = 'friend_no_apply',
}
AlertDataList.CodeToKey[320] = {
	kind = 'Tip',
	text = 'gift_no_help',
}
AlertDataList.CodeToKey[321] = {
	kind = 'Tip',
	text = 'gift_no_power',
}
AlertDataList.CodeToKey[322] = {
	kind = 'Tip',
	text = 'gift_no_give',
}
AlertDataList.CodeToKey[323] = {
	kind = 'Tip',
	text = 'gift_power_max',
}
AlertDataList.CodeToKey[324] = {
	kind = 'Tip',
	text = 'gift_get_max',
}
AlertDataList.CodeToKey[325] = {
	kind = 'Tip',
	text = 'gift_has_help',
}
AlertDataList.CodeToKey[326] = {
	kind = 'Tip',
	text = 'gift_not_self',
}
AlertDataList.CodeToKey[400] = {
	kind = 'Tip',
	text = 'grade_has_gain',
}
AlertDataList.CodeToKey[401] = {
	kind = 'Tip',
	text = 'grade_not_finish',
}
AlertDataList.CodeToKey[501] = {
	kind = 'Tip',
	text = 'not_enough_money',
}
AlertDataList.CodeToKey[502] = {
	kind = 'Tip',
	text = 'site_full',
}
AlertDataList.CodeToKey[503] = {
	kind = 'Tip',
	text = 'fragment_not_enough',
}
AlertDataList.CodeToKey[504] = {
	kind = 'Tip',
	text = 'not_has_fragment',
}
AlertDataList.CodeToKey[505] = {
	kind = 'Tip',
	text = 'temple_drop_cfg',
}
AlertDataList.CodeToKey[506] = {
	kind = 'Tip',
	text = 'temple_exc_cfg',
}
AlertDataList.CodeToKey[600] = {
	kind = 'Tip',
	text = 'legion_has_join',
}
AlertDataList.CodeToKey[601] = {
	kind = 'Tip',
	text = 'legion_not_join',
}
AlertDataList.CodeToKey[602] = {
	kind = 'Tip',
	text = 'legion_taget_has',
}
AlertDataList.CodeToKey[603] = {
	kind = 'Tip',
	text = 'legion_create_lev',
}
AlertDataList.CodeToKey[604] = {
	kind = 'Tip',
	text = 'legion_create_cost',
}
AlertDataList.CodeToKey[605] = {
	kind = 'Tip',
	text = 'legion_meb_max',
}
AlertDataList.CodeToKey[606] = {
	kind = 'Tip',
	text = 'legion_not_find',
}
AlertDataList.CodeToKey[607] = {
	kind = 'Tip',
	text = 'legion_no_meb',
}
AlertDataList.CodeToKey[608] = {
	kind = 'Tip',
	text = 'legion_join_lev',
}
AlertDataList.CodeToKey[609] = {
	kind = 'Tip',
	text = 'legion_has_apply',
}
AlertDataList.CodeToKey[610] = {
	kind = 'Tip',
	text = 'legion_no_apply',
}
AlertDataList.CodeToKey[611] = {
	kind = 'Tip',
	text = 'legion_has_invite',
}
AlertDataList.CodeToKey[612] = {
	kind = 'Tip',
	text = 'legion_no_invite',
}
AlertDataList.CodeToKey[613] = {
	kind = 'Tip',
	text = 'legion_not_right',
}
AlertDataList.CodeToKey[614] = {
	kind = 'Tip',
	text = 'legion_money_max',
}
AlertDataList.CodeToKey[615] = {
	kind = 'Tip',
	text = 'legion_gold_max',
}
AlertDataList.CodeToKey[616] = {
	kind = 'Tip',
	text = 'legion_config_error',
}
AlertDataList.CodeToKey[617] = {
	kind = 'Tip',
	text = 'legion_lev_max',
}
AlertDataList.CodeToKey[618] = {
	kind = 'Tip',
	text = 'legion_goods_lev',
}
AlertDataList.CodeToKey[619] = {
	kind = 'Tip',
	text = 'legion_id_error',
}
AlertDataList.CodeToKey[620] = {
	kind = 'Tip',
	text = 'legion_challenge_cd_limit',
}
AlertDataList.CodeToKey[621] = {
	kind = 'Tip',
	text = 'legion_challenge_bad_id',
}
AlertDataList.CodeToKey[622] = {
	kind = 'Tip',
	text = 'legion_devote_money',
}
AlertDataList.CodeToKey[623] = {
	kind = 'Tip',
	text = 'legion_devote_gold',
}
AlertDataList.CodeToKey[624] = {
	kind = 'Tip',
	text = 'legion_devote_less',
}
AlertDataList.CodeToKey[625] = {
	kind = 'Tip',
	text = 'legion_challenge_cnt_limit',
}
AlertDataList.CodeToKey[626] = {
	kind = 'Tip',
	text = 'legion_name_redo',
}
AlertDataList.CodeToKey[627] = {
	kind = 'Tip',
	text = 'legion_challenge_same',
}
AlertDataList.CodeToKey[628] = {
	kind = 'Tip',
	text = 'legion_del_more',
}
AlertDataList.CodeToKey[700] = {
	kind = 'Tip',
	text = 'market_no_config',
}
AlertDataList.CodeToKey[701] = {
	kind = 'Tip',
	text = 'market_no_reason',
}
AlertDataList.CodeToKey[702] = {
	kind = 'Tip',
	text = 'market_no_cost',
}
AlertDataList.CodeToKey[703] = {
	kind = 'Tip',
	text = 'market_config_error',
}
AlertDataList.CodeToKey[704] = {
	kind = 'Tip',
	text = 'sign_not_this',
}
AlertDataList.CodeToKey[705] = {
	kind = 'Tip',
	text = 'sign_has_gain',
}
AlertDataList.CodeToKey[710] = {
	kind = 'Tip',
	text = 'gvg_attack_less',
}
AlertDataList.CodeToKey[711] = {
	kind = 'Tip',
	text = 'gvg_no_player',
}
AlertDataList.CodeToKey[712] = {
	kind = 'Tip',
	text = 'gvg_fight_type_error',
}
AlertDataList.CodeToKey[713] = {
	kind = 'Tip',
	text = 'gvg_not_battle',
}
AlertDataList.CodeToKey[714] = {
	kind = 'Tip',
	text = 'gvg_target_not_battle',
}
AlertDataList.CodeToKey[715] = {
	kind = 'Tip',
	text = 'gvg_target_error',
}
AlertDataList.CodeToKey[716] = {
	kind = 'Tip',
	text = 'gvg_shield_destroy',
}
AlertDataList.CodeToKey[717] = {
	kind = 'Tip',
	text = 'gvg_shield_not_destroy',
}
AlertDataList.CodeToKey[718] = {
	kind = 'Tip',
	text = 'gvg_match_fail',
}
AlertDataList.CodeToKey[719] = {
	kind = 'Tip',
	text = 'gvg_battle_begin',
}
AlertDataList.CodeToKey[720] = {
	kind = 'Tip',
	text = 'gvg_battle_has_jion',
}
AlertDataList.CodeToKey[721] = {
	kind = 'Tip',
	text = 'gvg_not_start',
}
AlertDataList.CodeToKey[722] = {
	kind = 'Tip',
	text = 'gvg_no_time',
}
AlertDataList.CodeToKey[723] = {
	kind = 'Tip',
	text = 'gvg_attack_max',
}
AlertDataList.CodeToKey[724] = {
	kind = 'Tip',
	text = 'gvg_buy_shield_type',
}
AlertDataList.CodeToKey[725] = {
	kind = 'Tip',
	text = 'gvg_not_legion',
}

function AlertDataList:GetTextId(key)
	return self.DataList[key]
end

function AlertDataList:GetTextIdFromCode(code)
	return self:GetTextId(self.CodeToKey[code].text)
end

function AlertDataList:GetTypeFromCode(code)
	return self.CodeToKey[code].kind
end
