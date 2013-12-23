﻿<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="SynapseServer" generation="1" functional="0" release="0" Id="293b4f2d-0c88-42f5-9901-7e7107876bb8" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="SynapseServerGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="PageRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/SynapseServer/SynapseServerGroup/LB:PageRole:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="WorkerRole:StreetPass" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/SynapseServer/SynapseServerGroup/LB:WorkerRole:StreetPass" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="PageRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SynapseServer/SynapseServerGroup/MapPageRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="PageRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SynapseServer/SynapseServerGroup/MapPageRoleInstances" />
          </maps>
        </aCS>
        <aCS name="WorkerRole:ConnectionLimit" defaultValue="">
          <maps>
            <mapMoniker name="/SynapseServer/SynapseServerGroup/MapWorkerRole:ConnectionLimit" />
          </maps>
        </aCS>
        <aCS name="WorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SynapseServer/SynapseServerGroup/MapWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SynapseServer/SynapseServerGroup/MapWorkerRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:PageRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/SynapseServer/SynapseServerGroup/PageRole/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:WorkerRole:StreetPass">
          <toPorts>
            <inPortMoniker name="/SynapseServer/SynapseServerGroup/WorkerRole/StreetPass" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapPageRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SynapseServer/SynapseServerGroup/PageRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapPageRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SynapseServer/SynapseServerGroup/PageRoleInstances" />
          </setting>
        </map>
        <map name="MapWorkerRole:ConnectionLimit" kind="Identity">
          <setting>
            <aCSMoniker name="/SynapseServer/SynapseServerGroup/WorkerRole/ConnectionLimit" />
          </setting>
        </map>
        <map name="MapWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SynapseServer/SynapseServerGroup/WorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SynapseServer/SynapseServerGroup/WorkerRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="PageRole" generation="1" functional="0" release="0" software="d:\mydocument\visual studio 2012\Projects\SynapseServer\SynapseServer\csx\Debug\roles\PageRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;PageRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;PageRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;WorkerRole&quot;&gt;&lt;e name=&quot;StreetPass&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SynapseServer/SynapseServerGroup/PageRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/SynapseServer/SynapseServerGroup/PageRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/SynapseServer/SynapseServerGroup/PageRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="WorkerRole" generation="1" functional="0" release="0" software="d:\mydocument\visual studio 2012\Projects\SynapseServer\SynapseServer\csx\Debug\roles\WorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="StreetPass" protocol="http" portRanges="4724" />
            </componentports>
            <settings>
              <aCS name="ConnectionLimit" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;PageRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;WorkerRole&quot;&gt;&lt;e name=&quot;StreetPass&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SynapseServer/SynapseServerGroup/WorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/SynapseServer/SynapseServerGroup/WorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/SynapseServer/SynapseServerGroup/WorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="PageRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="WorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="PageRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="WorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="PageRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="WorkerRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="05bc9816-36bb-4ef0-a370-9cd7f6c96e8e" ref="Microsoft.RedDog.Contract\ServiceContract\SynapseServerContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="959eea1d-1c39-4da0-a6af-49bc689bc7a4" ref="Microsoft.RedDog.Contract\Interface\PageRole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/SynapseServer/SynapseServerGroup/PageRole:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="c91d7abd-6846-4125-b4ad-ea9059561c51" ref="Microsoft.RedDog.Contract\Interface\WorkerRole:StreetPass@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/SynapseServer/SynapseServerGroup/WorkerRole:StreetPass" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>