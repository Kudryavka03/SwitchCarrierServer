# 一个用于切换运营商的API
 路由器上的操作：

 1.执行```cat /etc/iproute2/rt_tables``` 找一个没被占用的id
 ```
 128     prelocal
 255     local
 254     main
 253     default
 ```

 2.这里以252和欲创建一个名为“campus_net”的路由表为例，执行

 ```echo '252    campus_net'>>/etc/iproute2/rt_tables```

 3.确定好初始化静态路由的命令，加入路由器自启动
 
 ```
 ip route add 192.168.255.0/24 via 0.0.0.0 dev br-lan table campus_net
 ip route add default via 192.168.72.1 dev br-hlu_net table campus_net
 ```

 4.编辑source，更改SSH账号密码端口，更改对应的指令

 5.编译并启动

## 附：切换指令
 ```
 /api/current_ip        显示当前IP
 /api/current_carrier   显示当前IP的运营商
 /api/switch            切换当前IP的运营商
 /api/switch/ipaddr     切换ipaddr的运营商
 ```

 ##目前这些指令倒是满足我的日常使用了，如需其他api可以自己改src。