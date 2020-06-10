
<security>
    <requestFiltering>
        <requestLimits maxQueryString="2147483647" />
    </requestFiltering>
</security>

在web.config中的<system.webServer>节点中增加上面的内容，否则当请求的字符串过长时IIS会拒绝请求。