# ServiceMatter.ServiceModel

## Purpose

Providing a framework that 
- wraps an extensible and interceptible pipeline around all component calls and supports the concept of Ambient Context
- provides a uniform programming model for consumers of the interfaces implemented by components
- allows for easy, code based configuration
- allows for real-time configuration changes
- is light and fast

Instantiation of components should be delegated to the IOC container of choice.


- Configure which proxy to use (see Proxies)
- Enable easy configuration of Authentication, Authorization, PreInvoke, and PostInvoke handlers as well as Wrappers
- The order  is:
    - Authentication handlers, 
    - Authorization handlers, 
    - PreInvoke handlers, 
    - Wrappers, 
    - component call, 
    - PostInvoke handlers.

## Proxies

- Proxyies can be automatically discovered by convention: If interface I`SomeName` is implemented by a class `SomeName`...Proxy then this class is used unless multiple candidates are found.
- A proxyname convention for technologies will be implemented. E.g. `SomeName`WcfProxy for a WCF type proxy or `SomeName`SfProxy for Service Fabric proxy or `SomeName`Proxy for plain class
- 
