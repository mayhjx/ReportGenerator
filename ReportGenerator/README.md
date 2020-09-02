
<security>
    <requestFiltering>
        <requestLimits maxQueryString="2147483647" />
    </requestFiltering>
</security>

在web.config中的<system.webServer>节点中增加上面的内容，否则当请求的字符串过长时IIS会拒绝请求。
已修改为post方法。


20200902 离群值问题：
当发现1个离群值时，需要剔除后进行计算；
当发现大于1个离群值时，需要剔除后重新判断离群值个数，直到离群值<=1个，此时再进行计算，同时提示剔除哪些离群值


