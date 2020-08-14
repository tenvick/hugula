local dispatcher_type = {
    --网络消息类型
    NET_DISCONNECT = "net_disconnect", --断线
    NET_RECONNECT = "net_reconnect", --重连成功
    NET_CONNECT_ERROR = "net_connect_error", --链接失败
    NET_CONNECT_SUCCESS = "net_connnect_success", --链接成功
    LOGIN_PACKET_ACK = "login_packet_ack", --登录确认信息
    NET_CERTIFY = "net_certify"
}

return dispatcher_type
