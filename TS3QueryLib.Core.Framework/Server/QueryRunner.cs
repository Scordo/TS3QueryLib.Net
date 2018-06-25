using System;
using System.Collections.Generic;
using System.Linq;
using TS3QueryLib.Core.CommandHandling;
using TS3QueryLib.Core.Common;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Server.Entities;
using TS3QueryLib.Core.Server.Notification;
using TS3QueryLib.Core.Server.Responses;

namespace TS3QueryLib.Core.Server
{
    /// <summary>
    /// Class for running queries against teamspeak 3 server
    /// </summary>
    public class QueryRunner : QueryRunnerBase
    {
        #region Properties


        /// <summary>
        /// Provides access to events raised by notifications
        /// </summary>
        public Notifications Notifications { get; protected set; }
        /// <summary>
        /// Provides utility methods that are using the queryrunner methods under the hood to ease some often used functions.
        /// </summary>
        public QueryUtils Utils { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of QueryRunner using the provided dispatcher
        /// </summary>
        /// <param name="queryDispatcher">The dispatcher used to send commands</param>
        public QueryRunner(IQueryDispatcher queryDispatcher) : base(queryDispatcher)
        {
            Notifications = new Notifications(this);
            Utils = new QueryUtils(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Authenticates with the TeamSpeak 3 Server instance using given ServerQuery login credentials.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        public SimpleResponse Login(string username, string password)
        {
            if (username.IsNullOrTrimmedEmpty())
                throw new ArgumentException("username is null or trimmed empty", "username");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password is null or empty", "password");

            Command command = CommandName.Login.CreateCommand();
            command.AddParameter("client_login_name", username);
            command.AddParameter("client_login_password", password);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Deselects the active virtual server and logs out from the server instance.
        /// </summary>
        public SimpleResponse Logout()
        {
            return ResponseBase<SimpleResponse>.Parse(SendCommand(CommandName.Logout.CreateCommand()));
        }

        /// <summary>
        /// Displays the servers version information including platform and build number.
        /// </summary>
        public VersionResponse GetVersion()
        {
            return ResponseBase<VersionResponse>.Parse(SendCommand(CommandName.Version.CreateCommand()));
        }

        /// <summary>
        /// Displays detailed connection information about the server instance including uptime, number of virtual servers online, traffic information, etc.
        /// </summary>
        public HostInfoResponse GetHostInfo()
        {
            return ResponseBase<HostInfoResponse>.Parse(SendCommand(CommandName.HostInfo.CreateCommand()));
        }

        /// <summary>
        /// Displays the server instance configuration including database revision number, the file transfer port, default group IDs, etc.
        /// </summary>
        public InstanceInfoResponse GetInstanceInfo()
        {
            return ResponseBase<InstanceInfoResponse>.Parse(SendCommand(CommandName.InstanceInfo.CreateCommand()));
        }

        /// <summary>
        /// Changes the server instance configuration using given properties.
        /// </summary>
        public SimpleResponse EditServerInstance(ServerInstanceModification modificationInstance)
        {
            if (modificationInstance == null)
                throw new ArgumentNullException("modificationInstance");

            Command command = CommandName.InstanceEdit.CreateCommand();
            modificationInstance.AddToCommand(command);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of IP addresses used by the server instance on multi-homed machines.
        /// </summary>
        public ListResponse<string> GetBindingList()
        {
            return ResponseBase<ListResponse<string>>.Parse(SendCommand(CommandName.BindingList.CreateCommand()), "ip");
        }

        /// <summary>
        /// Selects the virtual server specified with virtualServerId to allow further interaction. The ServerQuery client will
        /// appear on the virtual server and acts like a real TeamSpeak 3 Client, except it's unable to send or receive voice
        /// data.
        /// </summary>
        /// <param name="virtualServerId">The id of the virtual server</param>
        public SimpleResponse SelectVirtualServerById(uint virtualServerId)
        {
            Command command = SharedCommandName.Use.CreateCommand();
            command.AddParameter("sid", virtualServerId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Selects the virtual server specified with virtualServerPort to allow further interaction. The ServerQuery client will
        /// appear on the virtual server and acts like a real TeamSpeak 3 Client, except it's unable to send or receive voice
        /// data.
        /// </summary>
        /// <param name="virtualServerPort">The port of the virtual server</param>
        public SimpleResponse SelectVirtualServerByPort(uint virtualServerPort)
        {
            Command command = SharedCommandName.Use.CreateCommand();
            command.AddParameter("port", virtualServerPort);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Gets a list of virtual servers with 3 properties for each virtual server: ID, Port and Status.
        /// </summary>
        public ListResponse<ServerListItemBase> GetServerListShort()
        {
            Command command = CommandName.ServerList.CreateCommand();
            command.AddOption("short");

            return ListResponse<ServerListItemBase>.Parse(SendCommand(command), ServerListItemBase.Parse);
        }


        public ListResponse<ServerListItem> GetServerList()
        {
            return GetServerList(false);
        }

        public ListResponse<ServerListItem> GetServerList(bool includeAll)
        {
            return GetServerList(includeAll, false, false);
        }

        /// <summary>
        /// Displays a list of virtual servers including their ID, status, number of clients online, etc. If you're using the -all
        /// option, the server will list all virtual servers stored in the database. This can be useful when multiple server
        /// instances with different machine IDs are using the same database. The machine ID is used to identify the
        /// server instance a virtual server is associated with.
        /// The status of a virtual server can be either online, none and virtual. While online and none are self-
        /// explanatory, virtual is a bit more complicated. Whenever you select a virtual server which is currently
        /// stopped, it will be started in virtual mode which means you are able to change its configuration, create
        /// channels or change permissions, but no regular TeamSpeak 3 Client can connect. As soon as the last
        /// ServerQuery client deselects the virtual server, its status will be changed back to none.
        /// </summary>
        /// <param name="includeRemoteServers">whether to get only local servers</param>
        /// <param name="includeUniqueId">whether to include the virtual servers unique id</param>
        /// <returns></returns>
        public ListResponse<ServerListItem> GetServerList(bool includeRemoteServers, bool includeUniqueId)
        {
            return GetServerList(includeRemoteServers, includeUniqueId, false);
        }

        public ListResponse<ServerListItem> GetServerList(bool includeRemoteServers, bool includeUniqueId, bool onlyOfflineServers)
        {
            return GetServerList(false, includeRemoteServers, includeUniqueId, onlyOfflineServers);
        }

        private ListResponse<ServerListItem> GetServerList(bool includeAll, bool includeRemoteServers, bool includeUniqueId, bool onlyOfflineServers)
        {
            Command command = CommandName.ServerList.CreateCommand();

            if (includeRemoteServers || includeAll)
                command.AddOption("all");

            if (includeUniqueId || includeAll)
                command.AddOption("uid");

            if (onlyOfflineServers)
                command.AddOption("onlyoffline");

            return ListResponse<ServerListItem>.Parse(SendCommand(command), ServerListItem.Parse);
        }

        /// <summary>
        /// Returns the id of the virtual server using the specified voice port
        /// </summary>
        /// <param name="virtualServerPort">the port</param>
        public SingleValueResponse<uint?> GetServerIdByPort(ushort virtualServerPort)
        {
            Command command = CommandName.ServerIdGetByPort.CreateCommand();
            command.AddParameter("virtualserver_port", virtualServerPort);

            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "server_id");
        }

        /// <summary>
        /// Deletes the virtual server specified with virtualServerId. Please note that only virtual servers in stopped state can be deleted.
        /// </summary>
        /// <param name="virtualServerId">the virtual server id</param>
        public SimpleResponse DeleteServer(uint virtualServerId)
        {
            Command command = CommandName.ServerDelete.CreateCommand();
            command.AddParameter("sid", virtualServerId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Creates a new virtual server using the given properties and displays its ID and initial administrator token. If
        /// virtualserver_port is not specified, the server will test for the first unused UDP port.
        /// The first virtual server will be running on UDP port 9987 by default. Subsequently started virtual servers will
        /// be running on increasing UDP port numbers.
        /// </summary>
        /// <param name="serverModification">the properties as class</param>
        public CreateServerResponse CreateServer(VirtualServerModification serverModification)
        {
            if (serverModification == null)
                throw new ArgumentNullException("serverModification");

            //if (serverModification.Name.IsNullOrTrimmedEmpty())
            //    throw new ArgumentException("Name of the virtual server must be set!");

            Command command = CommandName.Servercreate.CreateCommand();
            serverModification.AddToCommand(command);

            return ResponseBase<CreateServerResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Starts the virtual server specified with sid. Depending on your permissions, you're able to start either your
        /// own virtual server only or all virtual servers in the server instance.
        /// </summary>
        /// <param name="virtualServerId">the virtual server id</param>
        public SimpleResponse StartVirtualServer(uint virtualServerId)
        {
            Command command = CommandName.ServerStart.CreateCommand();
            command.AddParameter("sid", virtualServerId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Stops the virtual server specified with sid. Depending on your permissions, you're able to stop either your own
        /// virtual server only or all virtual servers in the server instance.
        /// </summary>
        /// <param name="virtualServerId">the virtual server id</param>
        public SimpleResponse StopVirtualServer(uint virtualServerId)
        {
            Command command = CommandName.ServerStop.CreateCommand();
            command.AddParameter("sid", virtualServerId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Stops the entire TeamSpeak 3 Server instance by shutting down the process.
        /// </summary>
        public SimpleResponse StopServerProcess()
        {
            return ResponseBase<SimpleResponse>.Parse(SendCommand(CommandName.ServerProcessStop.CreateCommand()));
        }

        /// <summary>
        /// Displays detailed configuration information about the selected virtual server including unique ID, number of
        /// clients online, configuration, etc.
        /// </summary>
        public ServerInfoResponse GetServerInfo()
        {
            return ResponseBase<ServerInfoResponse>.Parse(SendCommand(CommandName.ServerInfo.CreateCommand()));
        }

        /// <summary>
        /// Displays detailed connection information about the selected virtual server including uptime, traffic information, etc.
        /// </summary>
        public ConnectionInfoResponse GetServerConnectionInfo()
        {
            return ResponseBase<ConnectionInfoResponse>.Parse(SendCommand(CommandName.ServerRequestConnectionInfo.CreateCommand()));
        }

        /// <summary>
        /// Changes the selected virtual servers configuration using given properties.
        /// </summary>
        /// <param name="serverModification">the properties as class</param>
        public SimpleResponse EditServer(VirtualServerModification serverModification)
        {
            if (serverModification == null)
                throw new ArgumentNullException("serverModification");

            Command command = CommandName.ServerEdit.CreateCommand();
            serverModification.AddToCommand(command);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of server groups available. Depending on your permissions, the output may also contain global ServerQuery groups and template groups.
        /// </summary>
        public ListResponse<ServerGroup> GetServerGroupList()
        {
            return ListResponse<ServerGroup>.Parse(SendCommand(CommandName.ServerGroupList.CreateCommand()), ServerGroup.Parse);
        }

        /// <summary>
        /// Creates a new server group using the name specified with name and displays its ID.
        /// </summary>
        /// <param name="serverGroupName">Name of the new group to create</param>
        public SingleValueResponse<uint?> AddServerGroup(string serverGroupName)
        {
            return AddServerGroup(serverGroupName, null);
        }

        /// <summary>
        /// Creates a new server group using the name specified with name and displays its ID.
        /// </summary>
        /// <param name="serverGroupName">Name of the new group to create</param>
        /// <param name="groupType">Type of the new group to create</param>
        public SingleValueResponse<uint?> AddServerGroup(string serverGroupName, GroupDatabaseType groupType)
        {
            return AddServerGroup(serverGroupName, (GroupDatabaseType?)groupType);
        }

        private SingleValueResponse<uint?> AddServerGroup(string serverGroupName, GroupDatabaseType? groupType)
        {
            if (serverGroupName.IsNullOrTrimmedEmpty())
                throw new ArgumentException("serverGroupName is null or trimmed empty", "serverGroupName");

            Command command = CommandName.ServerGroupAdd.CreateCommand();
            command.AddParameter("name", serverGroupName);

            if (groupType.HasValue)
                command.AddParameter("type", (int)groupType);

            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "sgid");
        }



        public SimpleResponse DeleteServerGroup(uint serverGroupId)
        {
            return DeleteServerGroup(serverGroupId, false);
        }

        /// <summary>
        /// Deletes the server group specified with sgid. If force is set to 1, the server group will be deleted even if there are clients within
        /// </summary>
        /// <param name="serverGroupId">Id of the group to delete</param>
        /// <param name="forceDeleteWhenClientsExist">force deletion even when clients are assigned this group</param>
        public SimpleResponse DeleteServerGroup(uint serverGroupId, bool forceDeleteWhenClientsExist)
        {
            Command command = CommandName.ServerGroupDel.CreateCommand();
            command.AddParameter("sgid", serverGroupId);
            command.AddParameter("force", forceDeleteWhenClientsExist);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Changes the name of the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to rename</param>
        /// <param name="newName">the new name of the group</param>
        /// <returns></returns>
        public SimpleResponse RenameServerGroup(uint serverGroupId, string newName)
        {
            if (newName.IsNullOrTrimmedEmpty())
                throw new ArgumentException("newName is null or trimmed empty", "newName");

            Command command = CommandName.ServerGroupRename.CreateCommand();
            command.AddParameter("sgid", serverGroupId);
            command.AddParameter("name", newName);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of permissions assigned to the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to get permissions for</param>
        public ListResponse<Permission> GetServerGroupPermissionList(uint serverGroupId)
        {
            return GetServerGroupPermissionList(serverGroupId, false);
        }

        /// <summary>
        /// Displays a list of permissions assigned to the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to get permissions for</param>
        /// <param name="returnNameInsteadOfId">If set to true, the returned permissions have the Id property set to 0 and the name property will be filled with the permission name</param>
        public ListResponse<Permission> GetServerGroupPermissionList(uint serverGroupId, bool returnNameInsteadOfId)
        {
            Command command = CommandName.ServerGroupPermList.CreateCommand();
            command.AddParameter("sgid", serverGroupId);

            if (returnNameInsteadOfId)
                command.AddOption("permsid");

            return ListResponse<Permission>.Parse(SendCommand(command), Permission.Parse);
        }

        /// <summary>
        /// Adds the specified permission to the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">the id of the server group</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddServerGroupPermission(uint serverGroupId, Permission permission)
        {
            return AddServerGroupPermission(serverGroupId, permission, false);
        }

        /// <summary>
        /// Adds the specified permission to the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">the id of the server group</param>
        /// <param name="permission">the permission to add</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse AddServerGroupPermission(uint serverGroupId, Permission permission, bool continueOnError)
        {
            return AddServerGroupPermission(serverGroupId, new[] { permission }, continueOnError);
        }

        /// <summary>
        /// Adds a set of specified permissions to the server group specified with sgid. Multiple permissions can be added by providing the four parameters of each permission.
        /// </summary>
        /// <param name="serverGroupId">the id of the server group</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddServerGroupPermission(uint serverGroupId, IEnumerable<Permission> permissions)
        {
            return AddServerGroupPermission(serverGroupId, permissions, false);
        }

        /// <summary>
        /// Adds a set of specified permissions to the server group specified with sgid. Multiple permissions can be added by providing the four parameters of each permission.
        /// </summary>
        /// <param name="serverGroupId">the id of the server group</param>
        /// <param name="permissions">the permissions to add</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        /// <returns></returns>
        public SimpleResponse AddServerGroupPermission(uint serverGroupId, IEnumerable<Permission> permissions, bool continueOnError)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ServerGroupAddPerm.CreateCommand();
            command.AddParameter("sgid", serverGroupId);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            uint index = 0;
            foreach (Permission permission in permissions)
            {
                command.AddParameter("permid", permission.Id, index);
                command.AddParameter("permvalue", permission.Value, index);
                command.AddParameter("permnegated", permission.Negated, index);
                command.AddParameter("permskip", permission.Skip, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Adds the specified permission to the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">the id of the server group</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddServerGroupPermission(uint serverGroupId, NamedPermission permission)
        {
            return AddServerGroupPermission(serverGroupId, permission, false);
        }

        /// <summary>
        /// Adds the specified permission to the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">the id of the server group</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddServerGroupPermission(uint serverGroupId, NamedPermission permission, bool continueOnError)
        {
            return AddServerGroupPermission(serverGroupId, new[] { permission }, continueOnError);
        }

        /// <summary>
        /// Adds a set of specified permissions to the server group specified with sgid. Multiple permissions can be added by providing the four parameters of each permission.
        /// </summary>
        /// <param name="serverGroupId">the id of the server group</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddServerGroupPermission(uint serverGroupId, IEnumerable<NamedPermission> permissions)
        {
            return AddServerGroupPermission(serverGroupId, permissions, false);
        }

        /// <summary>
        /// Adds a set of specified permissions to the server group specified with sgid. Multiple permissions can be added by providing the four parameters of each permission.
        /// </summary>
        /// <param name="serverGroupId">the id of the server group</param>
        /// <param name="permissions">the permissions to add</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse AddServerGroupPermission(uint serverGroupId, IEnumerable<NamedPermission> permissions, bool continueOnError)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ServerGroupAddPerm.CreateCommand();
            command.AddParameter("sgid", serverGroupId);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            uint index = 0;
            foreach (NamedPermission permission in permissions)
            {
                command.AddParameter("permsid", permission.Name, index);
                command.AddParameter("permvalue", permission.Value, index);
                command.AddParameter("permnegated", permission.Negated, index);
                command.AddParameter("permskip", permission.Skip, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Adds a set of specified permissions to *ALL* regular server groups on all virtual servers. The target groups will be identified by the value of their i_group_auto_update_type permission specified with sgtype. Multiple permissions can be added at once. A permission can be specified by permid or permsid. The known values for sgtype are: 10: Channel Guest 15: Server Guest 20: Query Guest 25: Channel Voice 30: Server Normal 35: Channel Operator 40: Channel Admin 45: Server Admin 50: Query Admin
        /// </summary>
        /// <param name="serverGroupType">the type of server group</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddServerGroupAutoPermission(ServerGroupType serverGroupType, IEnumerable<Permission> permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException(nameof(permissions));

            if (!permissions.Any())
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ServerGroupAutoAddPerm.CreateCommand();
            command.AddParameter("sgtype", (uint)serverGroupType);

            uint index = 0;
            foreach (Permission permission in permissions)
            {
                command.AddParameter("permid", permission.Id, index);
                command.AddParameter("permvalue", permission.Value, index);
                command.AddParameter("permnegated", permission.Negated, index);
                command.AddParameter("permskip", permission.Skip, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from *ALL* regular server groups on all virtual servers. The target groups will be identified by the value of their i_group_auto_update_type permission specified with sgtype. Multiple permissions can be removed at once. A permission can be specified by permid or permsid. The known values for sgtype are: 10: Channel Guest 15: Server Guest 20: Query Guest 25: Channel Voice 30: Server Normal 35: Channel Operator 40: Channel Admin 45: Server Admin 50: Query Admin
        /// </summary>
        /// <param name="serverGroupType">The server group type</param>
        /// <param name="permissionId">The id of the permission to remove</param>
        public SimpleResponse DeleteServerGroupAutoPermission(ServerGroupType serverGroupType, uint permissionId)
        {
            return DeleteServerGroupAutoPermissions(serverGroupType, new [] {permissionId});
        }

        /// <summary>
        /// Removes a set of specified permissions from *ALL* regular server groups on all virtual servers. The target groups will be identified by the value of their i_group_auto_update_type permission specified with sgtype. Multiple permissions can be removed at once. A permission can be specified by permid or permsid. The known values for sgtype are: 10: Channel Guest 15: Server Guest 20: Query Guest 25: Channel Voice 30: Server Normal 35: Channel Operator 40: Channel Admin 45: Server Admin 50: Query Admin
        /// </summary>
        /// <param name="serverGroupType">The server group type</param>
        /// <param name="permissionIdList">The ids of the permissions to remove</param>
        public SimpleResponse DeleteServerGroupAutoPermissions(ServerGroupType serverGroupType, IEnumerable<uint> permissionIdList)
        {
            if (permissionIdList == null)
                throw new ArgumentNullException(nameof(permissionIdList));

            if (!permissionIdList.Any())
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ServerGroupAutoDelPerm.CreateCommand();
            command.AddParameter("sgtype", (uint)serverGroupType);

            uint index = 0;
            foreach (uint permissionId in permissionIdList)
            {
                command.AddParameter("permid", permissionId, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from *ALL* regular server groups on all virtual servers. The target groups will be identified by the value of their i_group_auto_update_type permission specified with sgtype. Multiple permissions can be removed at once. A permission can be specified by permid or permsid. The known values for sgtype are: 10: Channel Guest 15: Server Guest 20: Query Guest 25: Channel Voice 30: Server Normal 35: Channel Operator 40: Channel Admin 45: Server Admin 50: Query Admin
        /// </summary>
        /// <param name="serverGroupType">The server group type</param>
        /// <param name="permissionName">The name of the permission to remove</param>
        public SimpleResponse DeleteServerGroupAutoPermission(ServerGroupType serverGroupType, string permissionName)
        {
            return DeleteServerGroupAutoPermissions(serverGroupType, new[] { permissionName });
        }

        /// <summary>
        /// Removes a set of specified permissions from *ALL* regular server groups on all virtual servers. The target groups will be identified by the value of their i_group_auto_update_type permission specified with sgtype. Multiple permissions can be removed at once. A permission can be specified by permid or permsid. The known values for sgtype are: 10: Channel Guest 15: Server Guest 20: Query Guest 25: Channel Voice 30: Server Normal 35: Channel Operator 40: Channel Admin 45: Server Admin 50: Query Admin
        /// </summary>
        /// <param name="serverGroupType">The server group type</param>
        /// <param name="permissionNameList">The names of the permissions to remove</param>
        public SimpleResponse DeleteServerGroupAutoPermissions(ServerGroupType serverGroupType, IEnumerable<string> permissionNameList)
        {
            if (permissionNameList == null)
                throw new ArgumentNullException(nameof(permissionNameList));

            if (!permissionNameList.Any())
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ServerGroupAutoDelPerm.CreateCommand();
            command.AddParameter("sgtype", (uint)serverGroupType);

            uint index = 0;
            foreach (string permissionName in permissionNameList)
            {
                command.AddParameter("permsid", permissionName, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes the specified permissions from the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to delete the permission from</param>
        /// <param name="permissionId">The id of the permission to remove</param>
        public SimpleResponse DeleteServerGroupPermission(uint serverGroupId, uint permissionId)
        {
            return DeleteServerGroupPermission(serverGroupId, permissionId, false);
        }

        /// <summary>
        /// Removes the specified permissions from the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to delete the permission from</param>
        /// <param name="permissionId">The id of the permission to remove</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse DeleteServerGroupPermission(uint serverGroupId, uint permissionId, bool continueOnError)
        {
            Command command = CommandName.ServerGroupDelPerm.CreateCommand();
            command.AddParameter("sgid", serverGroupId);
            command.AddParameter("permid", permissionId);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from the server group specified with sgid. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to delete the permissions from</param>
        /// <param name="permissionIdList">The ids of the permissions to remove</param>
        public SimpleResponse DeleteServerGroupPermissions(uint serverGroupId, IEnumerable<uint> permissionIdList)
        {
            return DeleteServerGroupPermissions(serverGroupId, permissionIdList, false);
        }

        /// <summary>
        /// Removes a set of specified permissions from the server group specified with sgid. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to delete the permissions from</param>
        /// <param name="permissionIdList">The ids of the permissions to remove</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse DeleteServerGroupPermissions(uint serverGroupId, IEnumerable<uint> permissionIdList, bool continueOnError)
        {
            if (permissionIdList == null)
                throw new ArgumentNullException("permissionIdList");

            if (permissionIdList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ServerGroupDelPerm.CreateCommand();
            command.AddParameter("sgid", serverGroupId);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            uint index = 0;
            foreach (uint permissionId in permissionIdList)
            {
                command.AddParameter("permid", permissionId, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes the specified permissions from the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to delete the permission from</param>
        /// <param name="permissionName">The name of the permission to remove</param>
        public SimpleResponse DeleteServerGroupPermission(uint serverGroupId, string permissionName)
        {
            return DeleteServerGroupPermission(serverGroupId, permissionName, false);
        }

        /// <summary>
        /// Removes the specified permissions from the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to delete the permission from</param>
        /// <param name="permissionName">The name of the permission to remove</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse DeleteServerGroupPermission(uint serverGroupId, string permissionName, bool continueOnError)
        {
            Command command = CommandName.ServerGroupDelPerm.CreateCommand();
            command.AddParameter("sgid", serverGroupId);
            command.AddParameter("permid", permissionName);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from the server group specified with sgid. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to delete the permissions from</param>
        /// <param name="permissionNameList">The names of the permissions to remove</param>
        public SimpleResponse DeleteServerGroupPermissions(uint serverGroupId, IEnumerable<string> permissionNameList)
        {
            return DeleteServerGroupPermissions(serverGroupId, permissionNameList, false);
        }

        /// <summary>
        /// Removes a set of specified permissions from the server group specified with sgid. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="serverGroupId">The id of the group to delete the permissions from</param>
        /// <param name="permissionNameList">The names of the permissions to remove</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse DeleteServerGroupPermissions(uint serverGroupId, IEnumerable<string> permissionNameList, bool continueOnError)
        {
            if (permissionNameList == null)
                throw new ArgumentNullException("permissionNameList");

            if (permissionNameList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ServerGroupDelPerm.CreateCommand();
            command.AddParameter("sgid", serverGroupId);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            uint index = 0;
            foreach (string permissionName in permissionNameList)
            {
                command.AddParameter("permsid", permissionName, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Adds a client to the server group specified with sgid. Please note that a client cannot be added to default groups or template groups.
        /// </summary>
        /// <param name="serverGroupId">The id of the server group</param>
        /// <param name="clientDatabaseId">The database id of the client</param>
        public SimpleResponse AddClientToServerGroup(uint serverGroupId, uint clientDatabaseId)
        {
            Command command = CommandName.ServerGroupAddClient.CreateCommand();
            command.AddParameter("sgid", serverGroupId);
            command.AddParameter("cldbid", clientDatabaseId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a client from the server group specified with sgid.
        /// </summary>
        /// <param name="serverGroupId">The id of the server group</param>
        /// <param name="clientDatabaseId">The database id of the client</param>
        public SimpleResponse DeleteClientFromServerGroup(uint serverGroupId, uint clientDatabaseId)
        {
            Command command = CommandName.ServerGroupDelClient.CreateCommand();
            command.AddParameter("sgid", serverGroupId);
            command.AddParameter("cldbid", clientDatabaseId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public ListResponse<ServerGroupClient> GetServerGroupClientList(uint serverGroupId)
        {
            return GetServerGroupClientList(serverGroupId, false);
        }

        /// <summary>
        /// Displays the IDs of all clients currently residing in the server group specified with sgid. If you're using the
        /// -names option, the output will also contain the last known nickname and the unique identifier of the clients.
        /// </summary>
        /// <param name="serverGroupId">The id of the server group</param>
        /// <param name="includeNicknamesAndUid">whether to include nickname and unique id</param>
        public ListResponse<ServerGroupClient> GetServerGroupClientList(uint serverGroupId, bool includeNicknamesAndUid)
        {
            Command command = CommandName.ServerGroupClientList.CreateCommand();
            command.AddParameter("sgid", serverGroupId);

            if (includeNicknamesAndUid)
                command.AddOption("names");

            return ListResponse<ServerGroupClient>.Parse(SendCommand(command), ServerGroupClient.Parse);
        }

        /// <summary>
        /// Displays all server groups the client specified with cldbid is currently residing in.
        /// </summary>
        /// <param name="clientDatabaseId">the clients database id</param>
        public ListResponse<ServerGroupLight> GetServerGroupsByClientId(uint clientDatabaseId)
        {
            Command command = CommandName.ServerGroupsByClientId.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId); // think this is wrong, should be cldbid

            return ListResponse<ServerGroupLight>.Parse(SendCommand(command), ServerGroupLight.Parse);
        }

        /// <summary>
        /// Displays a snapshot of the selected virtual server containing all settings, groups and known client identities.
        /// The data from a server snapshot can be used to restore a virtual servers configuration, channels and
        /// permissions using the serversnapshotdeploy command.
        /// </summary>
        public SimpleResponse GetServerSnapshot()
        {
            return ResponseBase<SimpleResponse>.Parse(SendCommand(CommandName.ServerSnapshotCreate.CreateCommand()));
        }

        /// <summary>
        /// Restores the selected virtual servers configuration using the data from a previously created server snapshot.
        /// Please note that the TeamSpeak 3 Server does NOT check for necessary permissions while deploying a
        /// snapshot so the command could be abused to gain additional privileges.
        /// </summary>
        /// <param name="snapshotData">The data that was got by calling GetServerSnapshot()</param>
        public SimpleResponse DeployServerSnapshot(string snapshotData)
        {
            if (snapshotData.IsNullOrTrimmedEmpty())
                throw new ArgumentException("snapshotData is null or trimmed empty", "snapshotData");

            Command command = CommandName.ServerSnapshotDeploy.CreateCommand();
            command.AddRaw(snapshotData);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public SimpleResponse RegisterForNotifications(ServerNotifyRegisterEvent eventSource)
        {
            return RegisterForNotifications(eventSource, null);
        }

        /// <summary>
        /// Registers for a specified category of events on a virtual server to receive notification messages. Depending on
        /// the notifications you've registered for, the server will send you a message on every event in the view of your
        /// ServerQuery client (e.g. clients joining your channel, incoming text messages, server configuration changes,
        /// etc). The event source is declared by the event parameter while id can be used to limit the notifications to a specific channel.
        /// </summary>
        /// <param name="eventSource">the event to register for</param>
        /// <param name="channelId">the optional channel id</param>
        public SimpleResponse RegisterForNotifications(ServerNotifyRegisterEvent eventSource, uint? channelId)
        {
            Command command = CommandName.ServerNotifyRegister.CreateCommand();
            command.AddParameter("event", eventSource.ToString().ToLower());

            if (channelId.HasValue)
                command.AddParameter("id", channelId.Value);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Unregisters all events previously registered with servernotifyregister so you will no longer receive notification messages.
        /// </summary>
        public SimpleResponse UnregisterNotifications()
        {
            return ResponseBase<SimpleResponse>.Parse(SendCommand(CommandName.ServerNotifyUnregister.CreateCommand()));
        }

        /// <summary>
        /// Sends a text message to all clients on all virtual servers in the TeamSpeak 3 Server instance.
        /// </summary>
        /// <param name="message">the message to send</param>
        public SimpleResponse SendGlobalMessage(string message)
        {
            if (message.IsNullOrTrimmedEmpty())
                throw new ArgumentException("message is null or trimmed empty", "message");

            Command command = CommandName.Gm.CreateCommand();
            command.AddParameter("msg", message);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Sends a text message a specified target. The type of the target is determined by targetmode while target
        /// specifies the ID of the recipient, whether it be a virtual server, a channel or a client.
        /// </summary>
        /// <param name="target">The target of the message</param>
        /// <param name="targetId">The id of the target</param>
        /// <param name="message">The message</param>
        /// <returns></returns>
        public SimpleResponse SendTextMessage(MessageTarget target, uint targetId, string message)
        {
            if (message.IsNullOrTrimmedEmpty())
                throw new ArgumentException("message is null or trimmed empty", "message");

            Command command = CommandName.SendTextMessage.CreateCommand();
            command.AddParameter("targetmode", (uint) target);
            command.AddParameter("msg", message);
            command.AddParameter("target", targetId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a specified number of entries from the servers log. Use a combination of lines and beginPos
        /// to get all available LogEntries. Note that you first have to select a virtual server.
        /// </summary>
        /// <param name="lines">The max amount of entries to retrieve. (max. 100)</param>
        /// <param name="beginPos">Must be 0 or the last_pos value of the previous log query response.</param>
        public ListResponse<LogEntry> GetLogEntries(ushort lines, uint beginPos = 0)
        {
            return GetLogEntries(lines, beginPos, false, false);
        }

        /// <summary>
        /// Displays a specified number of entries from the servers log. Depending on your permissions, you'll receive
        /// entries from the server instance log and/or your virtual server log. Using a combination of the comparator and
        /// timestamp parameters allows you to filter for log entries based on a specific date/time.
        /// </summary>
        /// <param name="lines">The max amount of entries to retrieve. (max. 100)</param>
        /// <param name="beginPos">Must be 0 or the last_pos value of the previous log query response.</param>
        /// <param name="reverse">Reverse start? (Not documented by TS)</param>
        /// <param name="instance">If set to true, the server will return lines from the master logfile (ts3server_0.log) instead of the selected virtual server logfile.</param>
        public ListResponse<LogEntry> GetLogEntries(ushort lines,uint beginPos,Boolean reverse,Boolean instance)
        {
            lines = Math.Max(Math.Min(lines, (ushort) 100), (ushort) 1);

            Command command = CommandName.LogView.CreateCommand();
            command.AddParameter("lines", lines);
            command.AddParameter("begin_pos", beginPos);
            command.AddParameter("instance", instance ? 1 : 0);
            command.AddParameter("reverse", reverse ? 1 : 0);

            return ListResponse<LogEntry>.Parse(SendCommand(command), LogEntry.Parse);
        }

        /// <summary>
        /// Writes a custom entry into the servers log. Depending on your permissions, you'll be able to add entries into
        /// the server instance log and/or your virtual servers log. The loglevel parameter specifies the type of the entry.
        /// </summary>
        /// <param name="logEntry">The logentry to add</param>
        public SimpleResponse AddLogEntry(LogEntryLight logEntry)
        {
            if (logEntry == null)
                throw new ArgumentNullException("logEntry");

            Command command = CommandName.LogAdd.CreateCommand();
            command.AddParameter("loglevel", (uint)logEntry.LogLevel);
            command.AddParameter("logmsg", logEntry.Message);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Writes a custom entry into the servers log. Depending on your permissions, you'll be able to add entries into
        /// the server instance log and/or your virtual servers log. The loglevel parameter specifies the type of the entry.
        /// </summary>
        /// <param name="logLevel">The loglevel</param>
        /// <param name="message">The message</param>
        /// <returns></returns>
        public SimpleResponse AddLogEntry(LogLevel logLevel, string message)
        {
            return AddLogEntry(new LogEntryLight(logLevel, message));
        }

        public ListResponse<ChannelListEntry> GetChannelList()
        {
            return GetChannelList(false);
        }

        public ListResponse<ChannelListEntry> GetChannelList(bool includeAll)
        {
            return GetChannelList(includeAll, false, false, false, false, false);
        }

        /// <summary>
        /// Displays a list of channels created on a virtual server including their ID, order, name, etc. The output can be modified using several command options.
        /// </summary>
        /// <param name="includeTopics">if set to true topic is included</param>
        /// <param name="includeFlags">if set to true flag parameters are included</param>
        /// <param name="includeVoiceInfo">if set to true voice parameters are included</param>
        /// <param name="includeLimits">if set to true limit parameters are included</param>
        /// <param name="includeIcon">if set to true icon parameter is included</param>
        public ListResponse<ChannelListEntry> GetChannelList(bool includeTopics, bool includeFlags, bool includeVoiceInfo, bool includeLimits, bool includeIcon)
        {
            return GetChannelList(false, includeTopics, includeFlags, includeVoiceInfo, includeLimits, includeIcon);
        }

        private ListResponse<ChannelListEntry> GetChannelList(bool includeAll, bool includeTopics, bool includeFlags, bool includeVoiceInfo, bool includeLimits, bool includeIcon)
        {
            Command command = CommandName.ChannelList.CreateCommand();

            if (includeTopics || includeAll)
                command.AddOption("topic");

            if (includeFlags || includeAll)
                command.AddOption("flags");

            if (includeVoiceInfo || includeAll)
                command.AddOption("voice");

            if (includeLimits || includeAll)
                command.AddOption("limits");

            if (includeIcon || includeAll)
                command.AddOption("icon");

            return ListResponse<ChannelListEntry>.Parse(SendCommand(command), ChannelListEntry.Parse);
        }



        /// <summary>
        /// Displays detailed configuration information about a channel including ID, topic, description, etc.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        public ChannelInfoResponse GetChannelInfo(uint channelId)
        {
            Command command = CommandName.ChannelInfo.CreateCommand();
            command.AddParameter("cid", channelId);

            return ResponseBase<ChannelInfoResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of channels matching a given name pattern.
        /// </summary>
        /// <param name="pattern">The pattern used for searchinf</param>
        public ListResponse<ChannelFindEntry> FindChannels(string pattern /* = null*/)
        {
            Command command = CommandName.ChannelFind.CreateCommand();

            if (!pattern.IsNullOrTrimmedEmpty())
                command.AddParameter("pattern", pattern);

            return ListResponse<ChannelFindEntry>.Parse(SendCommand(command), ChannelFindEntry.Parse);
        }

        public SimpleResponse MoveChannel(uint channelId, uint channelParentId)
        {
            return MoveChannel(channelId, channelParentId, null);
        }

        /// <summary>
        /// Moves a channel to a new parent channel with the ID cpid. If order is specified, the channel will be sorted right
        /// under the channel with the specified ID. If order is set to 0, the channel will be sorted right below the new parent.
        /// </summary>
        /// <param name="channelId">The id of the channel to move</param>
        /// <param name="channelParentId">The target parent channel id</param>
        /// <param name="order">the order used under the new target parent channel id</param>
        /// <returns></returns>
        public SimpleResponse MoveChannel(uint channelId, uint channelParentId, ushort? order)
        {
            Command command = CommandName.ChannelMove.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cpid", channelParentId);

            if (order.HasValue)
                command.AddParameter("order", order.Value);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Creates a new channel using the given properties and displays its ID.
        /// </summary>
        /// <param name="channelModification">the parameters of the channel as properties</param>
        public SingleValueResponse<uint?> CreateChannel(ChannelModification channelModification)
        {
            if (channelModification == null)
                throw new ArgumentNullException("channelModification");

            if (channelModification.Name.IsNullOrTrimmedEmpty())
                throw new ArgumentException("To create a channel you must at least provide a name for the channel.");

            Command command = CommandName.ChannelCreate.CreateCommand();
            channelModification.AddToCommand(command);
            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "cid");
        }

        /// <summary>
        /// Changes a channels configuration using given properties.
        /// </summary>
        /// <param name="channelId">the id of the channel to edit</param>
        /// <param name="channelModification">the parameters of the channel as properties</param>
        public SimpleResponse EditChannel(uint channelId, ChannelModification channelModification)
        {
            if (channelModification == null)
                throw new ArgumentNullException("channelModification");

            Command command = CommandName.ChannelEdit.CreateCommand();
            command.AddParameter("cid", channelId);
            channelModification.AddToCommand(command);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public SimpleResponse DeleteChannel(uint channelId)
        {
            return DeleteChannel(channelId, false);
        }

        /// <summary>
        /// Deletes an existing channel by ID. If force is set to 1, the channel will be deleted even if there are clients within.
        /// </summary>
        /// <param name="channelId">The id of the channel to delete</param>
        /// <param name="forceDeleteWhenClientsExist">delete even if clients are within the channel</param>
        public SimpleResponse DeleteChannel(uint channelId, bool forceDeleteWhenClientsExist)
        {
            Command command = CommandName.ChannelDelete.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("force", forceDeleteWhenClientsExist);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of permissions defined for a channel.
        /// </summary>
        /// <param name="channelId">The id of the channel to retrieve permissions for</param>
        public ListResponse<Permission> GetChannelPermissionList(uint channelId)
        {
            return GetChannelPermissionList(channelId, false);
        }

        /// <summary>
        /// Displays a list of permissions defined for a channel.
        /// </summary>
        /// <param name="channelId">The id of the channel to retrieve permissions for</param>
        /// <param name="returnNameInsteadOfId">If set to true, the returned permissions have the Id property set to 0 and the name property will be filled with the permission name</param>
        public ListResponse<Permission> GetChannelPermissionList(uint channelId, bool returnNameInsteadOfId)
        {
            Command command = CommandName.ChannelPermList.CreateCommand();
            command.AddParameter("cid", channelId);

            if (returnNameInsteadOfId)
                command.AddOption("permsid");

            return ListResponse<Permission>.Parse(SendCommand(command), Permission.Parse);
        }

        /// <summary>
        /// Adds the specified permission to the channel.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddChannelPermission(uint channelId, PermissionLight permission)
        {
            return AddChannelPermissions(channelId, new[] { permission });
        }

        /// <summary>
        /// Adds a set of specified permissions to a channel. Multiple permissions can be added by providing the two parameters of each permission.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddChannelPermissions(uint channelId, IEnumerable<PermissionLight> permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ChannelAddPerm.CreateCommand();
            command.AddParameter("cid", channelId);

            uint index = 0;
            foreach (PermissionLight permission in permissions)
            {
                command.AddParameter("permid", permission.Id, index);
                command.AddParameter("permvalue", permission.Value, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Adds the specified permission to the channel.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddChannelPermission(uint channelId, NamedPermissionLight permission)
        {
            return AddChannelPermissions(channelId, new[] { permission });
        }

        /// <summary>
        /// Adds a set of specified permissions to a channel. Multiple permissions can be added by providing the two parameters of each permission.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddChannelPermissions(uint channelId, IEnumerable<NamedPermissionLight> permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ChannelAddPerm.CreateCommand();
            command.AddParameter("cid", channelId);

            uint index = 0;
            foreach (NamedPermissionLight permission in permissions)
            {
                command.AddParameter("permsid", permission.Name, index);
                command.AddParameter("permvalue", permission.Value, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes the specified permissions from a channel.
        /// </summary>
        /// <param name="channelId">The id of the channel to delete the permission from</param>
        /// <param name="permissionId">The id of the permission to remove</param>
        public SimpleResponse DeleteChannelPermission(uint channelId, uint permissionId)
        {
            Command command = CommandName.ChannelDelPerm.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("permid", permissionId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from a channel. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="channelId">The id of the channel to delete the permissions from</param>
        /// <param name="permissionIdList">The ids of the permissions to remove</param>
        public SimpleResponse DeleteChannelPermissions(uint channelId, IEnumerable<uint> permissionIdList)
        {
            if (permissionIdList == null)
                throw new ArgumentNullException("permissionIdList");

            if (permissionIdList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ChannelDelPerm.CreateCommand();
            command.AddParameter("cid", channelId);

            uint index = 0;
            foreach (uint permissionId in permissionIdList)
            {
                command.AddParameter("permid", permissionId, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes the specified permissions from a channel.
        /// </summary>
        /// <param name="channelId">The id of the channel to delete the permission from</param>
        /// <param name="permissionName">The name of the permission to remove</param>
        public SimpleResponse DeleteChannelPermission(uint channelId, string permissionName)
        {
            Command command = CommandName.ChannelDelPerm.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("permsid", permissionName);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from a channel. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="channelId">The id of the channel to delete the permissions from</param>
        /// <param name="permissionNameList">The names of the permissions to remove</param>
        public SimpleResponse DeleteChannelPermissions(uint channelId, IEnumerable<string> permissionNameList)
        {
            if (permissionNameList == null)
                throw new ArgumentNullException("permissionNameList");

            if (permissionNameList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ChannelDelPerm.CreateCommand();
            command.AddParameter("cid", channelId);

            uint index = 0;
            foreach (string permissionName in permissionNameList)
            {
                command.AddParameter("permsid", permissionName, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of channel groups available on the selected virtual server.
        /// </summary>
        public ListResponse<ChannelGroup> GetChannelGroupList()
        {
            return ListResponse<ChannelGroup>.Parse(SendCommand(CommandName.ChannelGroupList.CreateCommand()), ChannelGroup.Parse);
        }

        /// <summary>
        /// Creates a new channel group using a given name and displays its ID.
        /// </summary>
        /// <param name="channelGroupName">the name of the group</param>
        public SingleValueResponse<uint?> AddChannelGroup(string channelGroupName)
        {
            return AddChannelGroup(channelGroupName, null);
        }

        /// <summary>
        /// Creates a new channel group using a given name and displays its ID.
        /// </summary>
        /// <param name="channelGroupName">the name of the group</param>
        /// <param name="groupType">the type of the group</param>
        public SingleValueResponse<uint?> AddChannelGroup(string channelGroupName, GroupDatabaseType groupType)
        {
            return AddChannelGroup(channelGroupName, (GroupDatabaseType?) groupType);
        }

        private SingleValueResponse<uint?> AddChannelGroup(string channelGroupName, GroupDatabaseType? groupType)
        {
            if (channelGroupName.IsNullOrTrimmedEmpty())
                throw new ArgumentException("channelGroupName is null or trimmed empty", "channelGroupName");

            Command command = CommandName.ChannelGroupAdd.CreateCommand();
            command.AddParameter("name", channelGroupName);

            if (groupType.HasValue)
                command.AddParameter("type", (int) groupType);

            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "cgid");
        }

        public SimpleResponse DeleteChannelGroup(uint channelGroupId)
        {
            return DeleteChannelGroup(channelGroupId, false);
        }

        /// <summary>
        /// Deletes a channel group by ID. If force is set to 1, the channel group will be deleted even if there are clients within.
        /// </summary>
        /// <param name="channelGroupId">the id of the channel group to delete</param>
        /// <param name="forceDeleteWhenClientsExist">delete even if there are members within the group</param>
        public SimpleResponse DeleteChannelGroup(uint channelGroupId, bool forceDeleteWhenClientsExist)
        {
            Command command = CommandName.ChannelGroupDel.CreateCommand();
            command.AddParameter("cgid", channelGroupId);
            command.AddParameter("force", forceDeleteWhenClientsExist);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Changes the name of a specified channel group.
        /// </summary>
        /// <param name="channelGroupId">the id of the channel group to rename</param>
        /// <param name="newName">the new channel group name</param>
        public SimpleResponse RenameChannelGroup(uint channelGroupId, string newName)
        {
            if (newName.IsNullOrTrimmedEmpty())
                throw new ArgumentException("newName is null or trimmed empty", "newName");

            Command command = CommandName.ChannelGroupRename.CreateCommand();
            command.AddParameter("cgid", channelGroupId);
            command.AddParameter("name", newName);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        ///  Adds the specified permission to a channel group.
        /// </summary>
        /// <param name="channelGroupId">the id of the channel group</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddChannelGroupPermission(uint channelGroupId, PermissionLight permission)
        {
            return AddChannelGroupPermissions(channelGroupId, new[] { permission });
        }

        /// <summary>
        /// Adds a set of specified permissions to a channel group. Multiple permissions can be added by providing the two parameters of each permission.
        /// </summary>
        /// <param name="channelGroupId">the id of the channel group</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddChannelGroupPermissions(uint channelGroupId, IEnumerable<PermissionLight> permissions)
        {
            return AddChannelGroupPermissions(channelGroupId, permissions, false);
        }

        /// <summary>
        /// Adds a set of specified permissions to a channel group. Multiple permissions can be added by providing the two parameters of each permission.
        /// </summary>
        /// <param name="channelGroupId">the id of the channel group</param>
        /// <param name="permissions">the permissions to add</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse AddChannelGroupPermissions(uint channelGroupId, IEnumerable<PermissionLight> permissions, bool continueOnError)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ChannelGroupAddPerm.CreateCommand();
            command.AddParameter("cgid", channelGroupId);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            uint index = 0;
            foreach (PermissionLight permission in permissions)
            {
                command.AddParameter("permid", permission.Id, index);
                command.AddParameter("permvalue", permission.Value, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        ///  Adds the specified permission to a channel group.
        /// </summary>
        /// <param name="channelGroupId">the id of the channel group</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddChannelGroupPermission(uint channelGroupId, NamedPermissionLight permission)
        {
            return AddChannelGroupPermissions(channelGroupId, new[] { permission });
        }

        /// <summary>
        /// Adds a set of specified permissions to a channel group. Multiple permissions can be added by providing the two parameters of each permission.
        /// </summary>
        /// <param name="channelGroupId">the id of the channel group</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddChannelGroupPermissions(uint channelGroupId, IEnumerable<NamedPermissionLight> permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ChannelGroupAddPerm.CreateCommand();
            command.AddParameter("cgid", channelGroupId);

            uint index = 0;
            foreach (NamedPermissionLight permission in permissions)
            {
                command.AddParameter("permsid", permission.Name, index);
                command.AddParameter("permvalue", permission.Value, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes the specified permissions from the channel group.
        /// </summary>
        /// <param name="channelGroupId">The id of the channel group to delete the permission from</param>
        /// <param name="permissionId">The id of the permission to remove</param>
        public SimpleResponse DeleteChannelGroupPermission(uint channelGroupId, uint permissionId)
        {
            return DeleteChannelGroupPermission(channelGroupId, permissionId, false);
        }

        /// <summary>
        /// Removes the specified permissions from the channel group.
        /// </summary>
        /// <param name="channelGroupId">The id of the channel group to delete the permission from</param>
        /// <param name="permissionId">The id of the permission to remove</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse DeleteChannelGroupPermission(uint channelGroupId, uint permissionId, bool continueOnError)
        {
            Command command = CommandName.ChannelGroupDelPerm.CreateCommand();
            command.AddParameter("cgid", channelGroupId);
            command.AddParameter("permid", permissionId);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from the channel group. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="channelGroupId">The id of the channel group to delete the permissions from</param>
        /// <param name="permissionIdList">The ids of the permissions to remove</param>
        public SimpleResponse DeleteChannelGroupPermissions(uint channelGroupId, IEnumerable<uint> permissionIdList)
        {
            if (permissionIdList == null)
                throw new ArgumentNullException("permissionIdList");

            if (permissionIdList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ChannelGroupDelPerm.CreateCommand();
            command.AddParameter("cgid", channelGroupId);

            uint index = 0;
            foreach (uint permissionId in permissionIdList)
            {
                command.AddParameter("permid", permissionId, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes the specified permissions from the channel group.
        /// </summary>
        /// <param name="channelGroupId">The id of the channel group to delete the permission from</param>
        /// <param name="permissionName">The name of the permission to remove</param>
        public SimpleResponse DeleteChannelGroupPermission(uint channelGroupId, string permissionName)
        {
            return DeleteChannelGroupPermission(channelGroupId, permissionName, false);
        }

        /// <summary>
        /// Removes the specified permissions from the channel group.
        /// </summary>
        /// <param name="channelGroupId">The id of the channel group to delete the permission from</param>
        /// <param name="permissionName">The name of the permission to remove</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse DeleteChannelGroupPermission(uint channelGroupId, string permissionName, bool continueOnError)
        {
            Command command = CommandName.ChannelGroupDelPerm.CreateCommand();
            command.AddParameter("cgid", channelGroupId);
            command.AddParameter("permsid", permissionName);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from the channel group. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="channelGroupId">The id of the channel group to delete the permissions from</param>
        /// <param name="permissionNameList">The names of the permissions to remove</param>
        public SimpleResponse DeleteChannelGroupPermissions(uint channelGroupId, IEnumerable<string> permissionNameList)
        {
            return DeleteChannelGroupPermissions(channelGroupId, permissionNameList, false);
        }

        /// <summary>
        /// Removes a set of specified permissions from the channel group. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="channelGroupId">The id of the channel group to delete the permissions from</param>
        /// <param name="permissionNameList">The names of the permissions to remove</param>
        /// <param name="continueOnError">if set to <c>true</c> continue on error.</param>
        public SimpleResponse DeleteChannelGroupPermissions(uint channelGroupId, IEnumerable<string> permissionNameList, bool continueOnError)
        {
            if (permissionNameList == null)
                throw new ArgumentNullException("permissionNameList");

            if (permissionNameList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ChannelGroupDelPerm.CreateCommand();
            command.AddParameter("cgid", channelGroupId);

            if (continueOnError)
                command.AddParameter("continueonerror", 1);

            uint index = 0;
            foreach (string permissionName in permissionNameList)
            {
                command.AddParameter("permsid", permissionName, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of permissions assigned to the channel group specified with cgid.
        /// </summary>
        /// <param name="channelGroupId">The channel group id</param>
        public ListResponse<Permission> GetChannelGroupPermissionList(uint channelGroupId)
        {
            return GetChannelGroupPermissionList(channelGroupId, false);
        }

        /// <summary>
        /// Displays a list of permissions assigned to the channel group specified with cgid.
        /// </summary>
        /// <param name="channelGroupId">The channel group id</param>
        /// <param name="returnNameInsteadOfId">If set to true, the returned permissions have the Id property set to 0 and the name property will be filled with the permission name</param>
        public ListResponse<Permission> GetChannelGroupPermissionList(uint channelGroupId, bool returnNameInsteadOfId)
        {
            Command command = CommandName.ChannelGroupPermList.CreateCommand();
            command.AddParameter("cgid", channelGroupId);

            if (returnNameInsteadOfId)
                command.AddOption("permsid");

            return ListResponse<Permission>.Parse(SendCommand(command), Permission.Parse);
        }

        public ListResponse<ChannelGroupClient> GetChannelGroupClientList()
        {
            return GetChannelGroupClientList(null, null, null);
        }

        /// <summary>
        /// Displays all the client and/or channel IDs currently assigned to channel groups. All three parameters are
        /// optional so you're free to choose the most suitable combination for your requirements.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <param name="clientDatabaseId">The client database id</param>
        /// <param name="channelGroupId">The channel group id</param>
        public ListResponse<ChannelGroupClient> GetChannelGroupClientList(uint? channelId, uint? clientDatabaseId, uint? channelGroupId)
        {
            Command command = CommandName.ChannelGroupClientList.CreateCommand();

            if (channelId.HasValue)
                command.AddParameter("cid", channelId.Value);

            if (clientDatabaseId.HasValue)
                command.AddParameter("cldbid", clientDatabaseId.Value);

            if (channelGroupId.HasValue)
                command.AddParameter("cgid", channelGroupId.Value);

            return ListResponse<ChannelGroupClient>.Parse(SendCommand(command), ChannelGroupClient.Parse);
        }

        /// <summary>
        /// Sets the channel group of a client to the ID specified with cgid.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <param name="clientDatabaseId">The client database id</param>
        /// <param name="channelGroupId">The channel group id</param>
        public SimpleResponse SetClientChannelGroup(uint channelGroupId, uint channelId, uint clientDatabaseId)
        {
            Command command = CommandName.SetClientChannelGroup.CreateCommand();
            command.AddParameter("cgid", channelGroupId);
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public ListResponse<ClientListEntry> GetClientList()
        {
            return GetClientList(false);
        }

        public ListResponse<ClientListEntry> GetClientList(bool includeAll)
        {
            return GetClientList(includeAll, false, false, false, false, false, false, false, false, false);
        }

        /// <summary>
        /// Displays a list of clients online on a virtual server including their ID, nickname, status flags, etc. The output can
        /// be modified using several command options. Please note that the output will only contain clients which are
        /// currently in channels you're able to subscribe to.
        /// </summary>
        /// <param name="includeUniqueId">if set to true, the unique id is included</param>
        /// <param name="includeAwayState">if set to true, the away info is included</param>
        /// <param name="includeVoiceInfo">if set tot true, the voice info is included</param>
        /// <param name="includeGroupInfo">if set to true, the group info is included</param>
        /// <param name="includeClientInfo">if set to true, the client info is included</param>
        /// <param name="includeTimes">if set to true, time information is included</param>
        /// <param name="includeIcon">if set to true, icon information is included</param>
        /// <param name="includeCountry">if set to true, country information is included</param>
        /// <param name="includeIPs">if set to true, ip information is included</param>
        public ListResponse<ClientListEntry> GetClientList(bool includeUniqueId, bool includeAwayState, bool includeVoiceInfo, bool includeGroupInfo, bool includeClientInfo, bool includeTimes, bool includeIcon, bool includeCountry, bool includeIPs)
        {
            return GetClientList(false, includeUniqueId, includeAwayState, includeVoiceInfo, includeGroupInfo, includeClientInfo, includeTimes, includeIcon, includeCountry, includeIPs);
        }

        private ListResponse<ClientListEntry> GetClientList(bool includeAll, bool includeUniqueId, bool includeAwayState, bool includeVoiceInfo, bool includeGroupInfo, bool includeClientInfo, bool includeTimes, bool includeIcon, bool includeCountry, bool includeIPs)
        {
            Command command = CommandName.ClientList.CreateCommand();

            if (includeUniqueId || includeAll)
                command.AddOption("uid");

            if (includeAwayState || includeAll)
                command.AddOption("away");

            if (includeVoiceInfo || includeAll)
                command.AddOption("voice");

            if (includeGroupInfo || includeAll)
                command.AddOption("groups");

            if (includeClientInfo || includeAll)
                command.AddOption("info");

            if (includeTimes || includeAll)
                command.AddOption("times");

            if (includeIcon || includeAll)
                command.AddOption("icon");

            if (includeCountry || includeAll)
                command.AddOption("country");

            if (includeIPs || includeAll)
                command.AddOption("ip");

            return ListResponse<ClientListEntry>.Parse(SendCommand(command), ClientListEntry.Parse);
        }

        /// <summary>
        /// Displays detailed configuration information about a client including unique ID, nickname, client version, etc.
        /// </summary>
        /// <param name="clientId">the client id</param>
        public ClientInfoResponse GetClientInfo(uint clientId)
        {
            Command command = CommandName.ClientInfo.CreateCommand();
            command.AddParameter("clid", clientId);

            return ResponseBase<ClientInfoResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of clients matching a given name pattern.
        /// </summary>
        /// <param name="pattern">the pattern used for searching</param>
        /// <returns></returns>
        public ListResponse<ClientFindEntry> FindClients(string pattern)
        {
            if (pattern.IsNullOrTrimmedEmpty())
                throw new ArgumentException("pattern is null or trimmed empty", "pattern");

            Command command = CommandName.ClientFind.CreateCommand();
            command.AddParameter("pattern", pattern);

            return ListResponse<ClientFindEntry>.Parse(SendCommand(command), ClientFindEntry.Parse);
        }

        /// <summary>
        /// Changes a clients settings using given properties.
        /// </summary>
        /// <param name="clientId">the id of the client to edit</param>
        /// <param name="modificationInstance">the modifications as class</param>
        public SimpleResponse EditClient(uint clientId, ClientModification modificationInstance)
        {
            if (modificationInstance == null)
                throw new ArgumentNullException("modificationInstance");

            Command command = CommandName.ClientEdit.CreateCommand();
            command.AddParameter("clid", clientId);
            modificationInstance.AddToCommand(command);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Gets the client database info.
        /// </summary>
        /// <param name="clientDatabaseId">The client database id.</param>
        public ClientDbInfoResponse GetClientDatabaseInfo(uint clientDatabaseId)
        {
            Command command = CommandName.ClientDbInfo.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            return ResponseBase<ClientDbInfoResponse>.Parse(SendCommand(command));
        }

        public ClientDbEntryListResponse GetClientDatabaseList()
        {
            return GetClientDatabaseList(null, null, false);
        }

        public ClientDbEntryListResponse GetClientDatabaseList(uint numberOfRecords)
        {
            return GetClientDatabaseList(null, numberOfRecords, false);
        }

        public ClientDbEntryListResponse GetClientDatabaseList(uint numberOfRecords, bool includeTotalCount)
        {
            return GetClientDatabaseList(null, numberOfRecords, includeTotalCount);
        }

        /// <summary>
        /// Displays a list of client identities known by the server including their database ID, last nickname, etc.
        /// </summary>
        /// <param name="startIndex">the index of the first record to return (starting  at 0). Default is 0.</param>
        /// <param name="numberOfRecords">The amount of records to return. Default is 25</param>
        /// <param name="includeTotalCount">If set to true, the property TotalClientsInDatabase of the response is filled with the whole amount of records in database.</param>
        public ClientDbEntryListResponse GetClientDatabaseList(uint? startIndex, uint? numberOfRecords, bool includeTotalCount)
        {
            Command command = CommandName.ClientDbList.CreateCommand();

            if (startIndex.HasValue)
                command.AddParameter("start", startIndex.Value);

            if (numberOfRecords.HasValue)
                command.AddParameter("duration", numberOfRecords.Value);

            if (includeTotalCount)
                command.AddOption("count");

            return ClientDbEntryListResponse.Parse(SendCommand(command));
        }

        public ListResponse<uint> FindClientDatabaseIds(string pattern)
        {
            return FindClientDatabaseIds(pattern, false);
        }

        /// <summary>
        /// Displays a list of client database IDs matching a given pattern. You can either search for a clients last known
        /// nickname or his unique identity by using the -uid option.
        /// </summary>
        /// <param name="pattern">The search pattern</param>
        /// <param name="useUniqueIdInsteadOfNicknameForSearch">if set to true, </param>
        public ListResponse<uint> FindClientDatabaseIds(string pattern, bool useUniqueIdInsteadOfNicknameForSearch)
        {
            if (pattern.IsNullOrTrimmedEmpty())
                throw new ArgumentException("pattern is null or trimmed empty", "pattern");

            Command command = CommandName.ClientDbFind.CreateCommand();
            command.AddParameter("pattern", pattern);

            if (useUniqueIdInsteadOfNicknameForSearch)
                command.AddOption("uid");

            return ResponseBase<ListResponse<uint>>.Parse(SendCommand(command), "cldbid");
        }

        /// <summary>
        /// Changes a clients settings using given properties.
        /// </summary>
        /// <param name="clientDatabaseId">the id of the database client to edit</param>
        /// <param name="modificationInstance">the modifications as class</param>
        public SimpleResponse EditDatbaseClient(int clientDatabaseId, ClientModification modificationInstance)
        {
            if (modificationInstance == null)
                throw new ArgumentNullException("modificationInstance");

            Command command = CommandName.ClientDbEdit.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);
            modificationInstance.AddToCommand(command);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Deletes a clients properties from the database.
        /// </summary>
        /// <param name="clientDatabaseId">Client database id</param>
        public SimpleResponse DeleteClientFromDatabase(uint clientDatabaseId)
        {
            Command command = CommandName.ClientDbDelete.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays all client IDs matching the unique identifier specified by cluid.
        /// </summary>
        /// <param name="clientUniqueId">the client unique id</param>
        public ListResponse<ClientIdEntry> GetClientIdsByUniqueId(string clientUniqueId)
        {
            if (clientUniqueId.IsNullOrTrimmedEmpty())
                throw new ArgumentException("clientUniqueId is null or trimmed empty", "clientUniqueId");

            Command command = CommandName.ClientGetIds.CreateCommand();
            command.AddParameter("cluid", clientUniqueId);

            return ListResponse<ClientIdEntry>.Parse(SendCommand(command), ClientIdEntry.Parse);
        }

        /// <summary>
        /// Displays the database ID and nickname matching the unique identifier specified by cluid.
        /// </summary>
        /// <param name="clientId">The client id</param>
        public ClientGetUidFromClidResponse GetUniqueIdFromClientId(uint clientId)
        {
            Command command = CommandName.ClientGetUidFromClid.CreateCommand();
            command.AddParameter("clid", clientId);

            return ResponseBase<ClientGetUidFromClidResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays the database ID matching the unique identifier specified by cluid.
        /// </summary>
        /// <param name="clientUniqueId">The client unique id</param>
        public ListResponse<uint> GetClientDatabaseIdsByUniqueId(string clientUniqueId)
        {
            if (clientUniqueId.IsNullOrTrimmedEmpty())
                throw new ArgumentException("clientUniqueId is null or trimmed empty", "clientUniqueId");

            Command command = CommandName.ClientGetDbIdFromUId.CreateCommand();
            command.AddParameter("cluid", clientUniqueId);

            return ResponseBase<ListResponse<uint>>.Parse(SendCommand(command), "cldbid");
        }

        /// <summary>
        /// Displays the database ID and nickname matching the unique identifier specified by cluid.
        /// </summary>
        /// <param name="clientUniqueId">The client unique id</param>
        public ClientGetNameFromUniqueIdResponse GetClientNameAndDatabaseIdByUniqueId(string clientUniqueId)
        {
            if (clientUniqueId.IsNullOrTrimmedEmpty())
                throw new ArgumentException("clientUniqueId is null or trimmed empty", "clientUniqueId");

            Command command = CommandName.ClientGetNameFromUId.CreateCommand();
            command.AddParameter("cluid", clientUniqueId);

            return ResponseBase<ClientGetNameFromUniqueIdResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays the unique identifier and nickname matching the database ID specified by cldbid.
        /// </summary>
        /// <param name="clientDatabaseId">client database id</param>
        public ClientGetNameFromDbIdResponse GetClientNameAndUniqueIdByClientDatabaseId(uint clientDatabaseId)
        {
            Command command = CommandName.ClientGetNameFromDbId.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            return ResponseBase<ClientGetNameFromDbIdResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Updates your own ServerQuery login credentials using a specified username. The password will be auto-generated.
        /// </summary>
        /// <param name="username">username to generate new a new password for</param>
        public SingleValueResponse<string> UpdateServerQueryLogin(string username)
        {
            if (username.IsNullOrTrimmedEmpty())
                throw new ArgumentException("username is null or trimmed empty", "username");

            Command command = CommandName.ClientSetServerQueryLogin.CreateCommand();
            command.AddParameter("client_login_name", username);

            return ResponseBase<SingleValueResponse<string>>.Parse(SendCommand(command), "client_login_password");
        }

        /// <summary>
        /// Change your ServerQuery clients settings using given properties.
        /// </summary>
        /// <param name="modificationInstance">the modifications as class</param>
        public SimpleResponse UpdateCurrentQueryClient(ClientModification modificationInstance)
        {
            if (modificationInstance == null)
                throw new ArgumentNullException("modificationInstance");

            Command command = SharedCommandName.ClientUpdate.CreateCommand();
            modificationInstance.AddToCommand(command);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public SimpleResponse MoveClient(uint clientId, uint channelId)
        {
            return MoveClient(clientId, channelId, null);
        }

        /// <summary>
        /// Moves one or more clients specified with clid to the channel with ID cid. If the target channel has a password,
        /// it needs to be specified with cpw. If the channel has no password, the parameter can be omitted.
        /// </summary>
        /// <param name="clientId">the id of the client to move</param>
        /// <param name="channelId">the id of the target channel</param>
        /// <param name="channelPassword">the password of the target channel</param>
        /// <returns></returns>
        public SimpleResponse MoveClient(uint clientId, uint channelId, string channelPassword)
        {
            Command command = CommandName.ClientMove.CreateCommand();
            command.AddParameter("clid", clientId);
            command.AddParameter("cid", channelId);

            if (channelPassword != null)
                command.AddParameter("cpw", channelPassword);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Moves the clients with the specified ids to the channel specified with id. The password is optional.
        /// </summary>
        /// <param name="clientIds">The client ids.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="channelPassword">The optional channel password.</param>
        public SimpleResponse MoveClients(IEnumerable<uint> clientIds, uint channelId, string channelPassword = null)
        {
            Command command = CommandName.ClientMove.CreateCommand();
            command.AddParameter("cid", channelId);

            if (channelPassword != null)
                command.AddParameter("cpw", channelPassword);

            uint index = 0;
            foreach (uint clientId in clientIds)
            {
                command.AddParameter("clid", clientId, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }


        public SimpleResponse KickClient(uint clientId, KickReason kickReason)
        {
            return KickClient(clientId, kickReason, null);
        }

        /// <summary>
        /// Kicks one client specified with clid from his currently joined channel or from the server,
        /// depending on reasonid. The reasonmsg parameter specifies a text message sent to the kicked client. This
        /// parameter is optional and may only have a maximum of 40 characters.
        /// </summary>
        /// <param name="clientId">the id of the client to kick</param>
        /// <param name="kickReason">the reason for kicking</param>
        /// <param name="reasonMessage">the reason for kicking as text</param>
        public SimpleResponse KickClient(uint clientId, KickReason kickReason, string reasonMessage)
        {
            return KickClients(new[] {clientId}, kickReason, reasonMessage);
        }

        public SimpleResponse KickClients(IEnumerable<uint> clientIds, KickReason kickreason)
        {
            return KickClients(clientIds, kickreason, null);
        }

        /// <summary>
        /// Kicks one or more clients specified with clid from their currently joined channel or from the server,
        /// depending on reasonid. The reasonmsg parameter specifies a text message sent to the kicked clients. This
        /// parameter is optional and may only have a maximum of 40 characters.
        /// </summary>
        /// <param name="clientIds">the ids of the clients to kick</param>
        /// <param name="kickreason">the reason for kicking</param>
        /// <param name="reasonMessage">the reason for kicking as text</param>
        public SimpleResponse KickClients(IEnumerable<uint> clientIds, KickReason kickreason, string reasonMessage)
        {
            Command command = CommandName.ClientKick.CreateCommand();

            foreach (uint clientId in clientIds)
                command.AddParameter("clid", clientId);

            command.AddParameter("reasonid", (uint) kickreason);

            if (!reasonMessage.IsNullOrTrimmedEmpty())
            {
                if (reasonMessage.Length > 40)
                    reasonMessage = reasonMessage.Substring(0, 40);

                command.AddParameter("reasonmsg", reasonMessage);
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Sends a poke message to the client specified with clid.
        /// </summary>
        /// <param name="clientId">The id of the client to poke</param>
        /// <param name="message">the message to sent</param>
        public SimpleResponse PokeClient(uint clientId, string message)
        {
            Command command = CommandName.ClientPoke.CreateCommand();
            command.AddParameter("clid", clientId);
            command.AddParameter("msg", message);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of permissions defined for a client.
        /// </summary>
        /// <param name="clientDatabaseId">the client database id</param>
        public ListResponse<Permission> GetClientPermissionList(uint clientDatabaseId)
        {
            return GetClientPermissionList(clientDatabaseId, false);
        }

        /// <summary>
        /// Displays a list of permissions defined for a client.
        /// </summary>
        /// <param name="clientDatabaseId">the client database id</param>
        /// <param name="returnNameInsteadOfId">If set to true, the returned permissions have the Id property set to 0 and the name property will be filled with the permission name</param>
        public ListResponse<Permission> GetClientPermissionList(uint clientDatabaseId, bool returnNameInsteadOfId)
        {
            Command command = CommandName.ClientPermList.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            if (returnNameInsteadOfId)
                command.AddOption("permsid");

            return ListResponse<Permission>.Parse(SendCommand(command), Permission.Parse);
        }

        /// <summary>
        /// Adds the specified permission to a client.
        /// </summary>
        /// <param name="clientDatabaseId">the database id of the client</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddClientPermission(uint clientDatabaseId, ClientPermission permission)
        {
            return AddClientPermissions(clientDatabaseId, new[] { permission });
        }

        /// <summary>
        /// Adds a set of specified permissions to a client. Multiple permissions can be added by providing the three parameters of each permission.
        /// </summary>
        /// <param name="clientDatabaseId">the database id of the client</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddClientPermissions(uint clientDatabaseId, IEnumerable<ClientPermission> permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ClientAddPerm.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            uint index = 0;
            foreach (ClientPermission permission in permissions)
            {
                command.AddParameter("permid", permission.Id, index);
                command.AddParameter("permvalue", permission.Value, index);
                command.AddParameter("permskip", permission.Skip, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Adds the specified permission to a client.
        /// </summary>
        /// <param name="clientDatabaseId">the database id of the client</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddClientPermission(uint clientDatabaseId, NamedClientPermission permission)
        {
            return AddClientPermissions(clientDatabaseId, new[] { permission });
        }

        /// <summary>
        /// Adds a set of specified permissions to a client. Multiple permissions can be added by providing the three parameters of each permission.
        /// </summary>
        /// <param name="clientDatabaseId">the database id of the client</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddClientPermissions(uint clientDatabaseId, IEnumerable<NamedClientPermission> permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ClientAddPerm.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            uint index = 0;
            foreach (NamedClientPermission permission in permissions)
            {
                command.AddParameter("permsid", permission.Name, index);
                command.AddParameter("permvalue", permission.Value, index);
                command.AddParameter("permskip", permission.Skip, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from a client. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="permissionId">the permission id</param>
        public SimpleResponse DeleteClientPermission(uint clientDatabaseId, uint permissionId)
        {
            Command command = CommandName.ClientDelPerm.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);
            command.AddParameter("permid", permissionId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from the channel group. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="permissionIdList">the permission ids</param>
        public SimpleResponse DeleteClientPermissions(uint clientDatabaseId, IEnumerable<uint> permissionIdList)
        {
            if (permissionIdList == null)
                throw new ArgumentNullException("permissionIdList");

            if (permissionIdList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ClientDelPerm.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            uint index = 0;
            foreach (uint permissionId in permissionIdList)
            {
                command.AddParameter("permid", permissionId, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from a client. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="permissionName">the permission id</param>
        public SimpleResponse DeleteClientPermission(uint clientDatabaseId, string permissionName)
        {
            Command command = CommandName.ClientDelPerm.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);
            command.AddParameter("permsid", permissionName);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes a set of specified permissions from the channel group. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="permissionNameList">the permission names</param>
        public SimpleResponse DeleteClientPermissions(uint clientDatabaseId, IEnumerable<string> permissionNameList)
        {
            if (permissionNameList == null)
                throw new ArgumentNullException("permissionNameList");

            if (permissionNameList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ClientDelPerm.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            uint index = 0;
            foreach (string permissionName in permissionNameList)
            {
                command.AddParameter("permsid", permissionName, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of permissions defined for a client in a specific channel.
        /// </summary>
        /// <param name="channelId">the id of the channel></param>
        /// <param name="clientDatabaseId">the clients database id</param>
        public ListResponse<Permission> GetChannelClientPermissionList(uint channelId, uint clientDatabaseId)
        {
            Command command = CommandName.ChannelClientPermList.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);

            return ListResponse<Permission>.Parse(SendCommand(command), Permission.Parse);
        }

        /// <summary>
        /// Displays a list of permissions defined for a client in a specific channel.
        /// </summary>
        /// <param name="channelId">the id of the channel></param>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="returnNameInsteadOfId">If set to true, the returned permissions have the Id property set to 0 and the name property will be filled with the permission name</param>
        public ListResponse<Permission> GetChannelClientPermissionList(uint channelId, uint clientDatabaseId, bool returnNameInsteadOfId)
        {
            Command command = CommandName.ChannelClientPermList.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);

            if (returnNameInsteadOfId)
                command.AddOption("permsid");

            return ListResponse<Permission>.Parse(SendCommand(command), Permission.Parse);
        }

        /// <summary>
        ///  Adds the specified permission to a client in a specific channel.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="clientDatabaseId">the database id of the client</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddChannelClientPermission(uint channelId, uint clientDatabaseId, PermissionLight permission)
        {
            return AddChannelClientPermissions(channelId,clientDatabaseId, new[] { permission });
        }

        /// <summary>
        /// Adds a set of specified permissions to a client in a specific channel. Multiple permissions can be added by providing the three parameters of each permission.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="clientDatabaseId">the database id of the client</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddChannelClientPermissions(uint channelId, uint clientDatabaseId, IEnumerable<PermissionLight> permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ChannelClientAddPerm.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);

            uint index = 0;
            foreach (PermissionLight permission in permissions)
            {
                command.AddParameter("permid", permission.Id, index);
                command.AddParameter("permvalue", permission.Value, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        ///  Adds the specified permission to a client in a specific channel.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="clientDatabaseId">the database id of the client</param>
        /// <param name="permission">the permission to add</param>
        public SimpleResponse AddChannelClientPermission(uint channelId, uint clientDatabaseId, NamedPermissionLight permission)
        {
            return AddChannelClientPermissions(channelId, clientDatabaseId, new[] { permission });
        }

        /// <summary>
        /// Adds a set of specified permissions to a client in a specific channel. Multiple permissions can be added by providing the three parameters of each permission.
        /// </summary>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="clientDatabaseId">the database id of the client</param>
        /// <param name="permissions">the permissions to add</param>
        public SimpleResponse AddChannelClientPermissions(uint channelId, uint clientDatabaseId, IEnumerable<NamedPermissionLight> permissions)
        {
            if (permissions == null)
                throw new ArgumentNullException("permissions");

            if (permissions.Count() == 0)
                throw new ArgumentException("permissions are empty.");

            Command command = CommandName.ChannelClientAddPerm.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);

            uint index = 0;
            foreach (NamedPermissionLight permission in permissions)
            {
                command.AddParameter("permsid", permission.Name, index);
                command.AddParameter("permvalue", permission.Value, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes the specified permission from the channel group.
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="permissionId">the permission id</param>
        public SimpleResponse DeleteChannelClientPermission(uint channelId, uint clientDatabaseId, uint permissionId)
        {
            return DeleteChannelClientPermissions(channelId, clientDatabaseId, new[] {permissionId});
        }

        /// <summary>
        /// Removes a set of specified permissions from the channel group. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="permissionIdList">the permission ids</param>
        public SimpleResponse DeleteChannelClientPermissions(uint channelId, uint clientDatabaseId, IEnumerable<uint> permissionIdList)
        {
            if (permissionIdList == null)
                throw new ArgumentNullException("permissionIdList");

            if (permissionIdList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ChannelClientDelPerm.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);

            uint index = 0;
            foreach (uint permissionId in permissionIdList)
            {
                command.AddParameter("permid", permissionId, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Removes the specified permission from the channel group.
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="permissionName">the permission name</param>
        public SimpleResponse DeleteChannelClientPermission(uint channelId, uint clientDatabaseId, string permissionName)
        {
            return DeleteChannelClientPermissions(channelId, clientDatabaseId, new[] { permissionName });
        }

        /// <summary>
        /// Removes a set of specified permissions from the channel group. Multiple permissions can be removed at once.
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="clientDatabaseId">the clients database id</param>
        /// <param name="permissionIdList">the permission names</param>
        public SimpleResponse DeleteChannelClientPermissions(uint channelId, uint clientDatabaseId, IEnumerable<string> permissionIdList)
        {
            if (permissionIdList == null)
                throw new ArgumentNullException("permissionIdList");

            if (permissionIdList.Count() == 0)
                throw new ArgumentException("permissions are empty");

            Command command = CommandName.ChannelClientDelPerm.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);

            uint index = 0;
            foreach (string permissionName in permissionIdList)
            {
                command.AddParameter("permsid", permissionName, index);
                index++;
            }

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays the current value of the permission specified with permid or permsid
        /// for your own connection. This can be useful when you need to check your own
        /// privileges.
        /// </summary>
        /// <param name="permid">the permission id</param>
        public OwnPermissionResponse GetOwnPermissionDetails(uint permid)
        {
            Command command = CommandName.PermGet.CreateCommand();
            command.AddParameter("permid", permid);

            return ResponseBase<OwnPermissionResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays the current value of the permission specified with permid or permsid
        /// for your own connection. This can be useful when you need to check your own
        /// privileges.
        /// </summary>
        /// <param name="permsid">the permission name</param>
        public OwnPermissionResponse GetOwnPermissionDetails(string permsid)
        {
            Command command = CommandName.PermGet.CreateCommand();
            command.AddParameter("permsid", permsid);

            return ResponseBase<OwnPermissionResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of permissions available on the server instance including ID, name and description.
        /// </summary>
        public ListResponse<PermissionDetails> GetPermissionList()
        {
            return ListResponse<PermissionDetails>.Parse(SendCommand(CommandName.PermissionList.CreateCommand()), PermissionDetails.Parse);
        }

        /// <summary>
        /// Returns a list of PermissionInfo objects matching the provided permission name
        /// </summary>
        /// <param name="permissionName">the permission name used for searching</param>
        public ListResponse<PermissionInfo> GetPermissionInfoByName(string permissionName)
        {
            return GetPermissionInfoByNames(new[] { permissionName });
        }

        /// <summary>
        /// Returns a list of PermissionInfo objects matching the provided permission names
        /// </summary>
        /// <param name="permissionNames">the name of the permissions used for searching</param>
        public ListResponse<PermissionInfo> GetPermissionInfoByNames(IEnumerable<string> permissionNames)
        {
            if (permissionNames == null)
                throw new ArgumentNullException("permissionNames");

            if (permissionNames.Count() == 0)
                throw new ArgumentException("permissionNames are empty");

            Command command = CommandName.PermIdGetByName.CreateCommand();

            uint index = 0;
            foreach (string permissionName in permissionNames)
            {
                command.AddParameter("permsid", permissionName, index);
                index++;
            }

            return ListResponse<PermissionInfo>.Parse(SendCommand(command), PermissionInfo.Parse);
        }

        /// <summary>
        /// Displays all permissions assigned to a client for the channel specified with cid. If permid is set to 0, all permissions will be displayed.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <param name="clientDatabaseId">The client database id</param>
        /// <param name="permissionId">the permission id</param>
        public ListResponse<PermissionOverviewEntry> GetPermissionOverview(uint channelId, uint clientDatabaseId, uint permissionId)
        {
            Command command = CommandName.PermOverview.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);
            command.AddParameter("permid", permissionId);

            return ListResponse<PermissionOverviewEntry>.Parse(SendCommand(command), PermissionOverviewEntry.Parse);
        }

        /// <summary>
        /// Displays all permissions assigned to a client for the channel specified with cid. If permid is set to 0, all permissions will be displayed.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <param name="clientDatabaseId">The client database id</param>
        /// <param name="permissionName">the permission name</param>
        public ListResponse<PermissionOverviewEntry> GetPermissionOverview(uint channelId, uint clientDatabaseId, string permissionName)
        {
            Command command = CommandName.PermOverview.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cldbid", clientDatabaseId);
            command.AddParameter("permsid", permissionName);

            return ListResponse<PermissionOverviewEntry>.Parse(SendCommand(command), PermissionOverviewEntry.Parse);
        }

        /// <summary>
        /// Displays the IDs of all clients, channels or groups using the permission specified with permid.
        /// </summary>
        /// <param name="permissionId">the id of the permission</param>
        public ListResponse<PermissionFindEntry> FindPermissions(uint permissionId)
        {
            Command command = CommandName.PermFind.CreateCommand();
            command.AddParameter("permid", permissionId);

            return ListResponse<PermissionFindEntry>.Parse(SendCommand(command), PermissionFindEntry.Parse);
        }

        /// <summary>
        /// Displays the IDs of all clients, channels or groups using the permission specified with permsid.
        /// </summary>
        /// <param name="permissionName">the name of the permission</param>
        public ListResponse<PermissionFindEntry> FindPermissions(string permissionName)
        {
            Command command = CommandName.PermFind.CreateCommand();
            command.AddParameter("permsid", permissionName);

            return ListResponse<PermissionFindEntry>.Parse(SendCommand(command), PermissionFindEntry.Parse);
        }

        /// <summary>
        /// Restores the default permission settings on the selected virtual server and creates a new initial administrator
        /// token. Please note that in case of an error during the permreset call - e.g. when the database has been modified
        /// or corrupted - the virtual server will be deleted from the database. The new admin token is returned by this method.
        /// </summary>
        public SingleValueResponse<string> ResetPermissions()
        {
            return ResponseBase<SingleValueResponse<string>>.Parse(SendCommand(CommandName.PermReset.CreateCommand()), "token");
        }

        /// <summary>
        /// Displays a list of privilege keys available including their type and group IDs. Privilege keys can be used to gain access to specified server or channel groups.
        /// </summary>
        public ListResponse<Token> GetPrivilegeKeyList()
        {
            return ListResponse<Token>.Parse(SendCommand(CommandName.PrivilegeKeyList.CreateCommand()), Token.Parse);
        }

        /// <summary>
        /// Create a new PrivilegeKey. If tokentype is set to 0, the ID specified with tokenid1 will be a server group ID. Otherwise,
        /// tokenid1 is used as a channel group ID and you need to provide a valid channel ID using tokenid2.
        /// </summary>
        /// <param name="isChannelGroupIdInsteadOfServerGroupId">if set to true, its a server group id</param>
        /// <param name="groupId">the id of the group</param>
        /// <param name="channelId">the id of the channel</param>
        public SingleValueResponse<string> AddPrivilegeKey(bool isChannelGroupIdInsteadOfServerGroupId, uint groupId, uint channelId)
        {
            return AddPrivilegeKey(isChannelGroupIdInsteadOfServerGroupId, groupId, channelId, (string) null);
        }

        /// <summary>
        /// Create a new PrivilegeKey. If tokentype is set to 0, the ID specified with tokenid1 will be a server group ID. Otherwise,
        /// tokenid1 is used as a channel group ID and you need to provide a valid channel ID using tokenid2.
        /// </summary>
        /// <param name="isChannelGroupIdInsteadOfServerGroupId">if set to true, its a server group id</param>
        /// <param name="groupId">the id of the group</param>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="tokenDescription">the description for the token</param>
        public SingleValueResponse<string> AddPrivilegeKey(bool isChannelGroupIdInsteadOfServerGroupId, uint groupId, uint channelId, string tokenDescription)
        {
            return AddPrivilegeKey(isChannelGroupIdInsteadOfServerGroupId, groupId, channelId, tokenDescription, (string) null);
        }

        /// <summary>
        /// Create a new PrivilegeKey. If tokentype is set to 0, the ID specified with tokenid1 will be a server group ID. Otherwise,
        /// tokenid1 is used as a channel group ID and you need to provide a valid channel ID using tokenid2.
        /// </summary>
        /// <param name="isChannelGroupIdInsteadOfServerGroupId">if set to true, its a server group id</param>
        /// <param name="groupId">the id of the group</param>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="identValuePairs">The  parameter allows you to specify a set of custom client properties. This feature can be used when generating tokens to combine a website account database with a TeamSpeak user.</param>
        public SingleValueResponse<string> AddPrivilegeKey(bool isChannelGroupIdInsteadOfServerGroupId, uint groupId, uint channelId, IEnumerable<KeyValuePair<string, string>> identValuePairs)
        {
            return AddPrivilegeKey(isChannelGroupIdInsteadOfServerGroupId, groupId, channelId, null, identValuePairs);
        }

        /// <summary>
        /// Create a new PrivilegeKey. If tokentype is set to 0, the ID specified with tokenid1 will be a server group ID. Otherwise,
        /// tokenid1 is used as a channel group ID and you need to provide a valid channel ID using tokenid2.
        /// </summary>
        /// <param name="isChannelGroupIdInsteadOfServerGroupId">if set to true, its a server group id</param>
        /// <param name="groupId">the id of the group</param>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="tokenDescription">the description for the token</param>
        /// <param name="identValuePairs">The  parameter allows you to specify a set of custom client properties. This feature can be used when generating tokens to combine a website account database with a TeamSpeak user.</param>
        public SingleValueResponse<string> AddPrivilegeKey(bool isChannelGroupIdInsteadOfServerGroupId, uint groupId, uint channelId, string tokenDescription, IEnumerable<KeyValuePair<string, string>> identValuePairs)
        {
            string tokenCustomSettings = string.Join("|", identValuePairs.Select(ivp => string.Format("ident={0} value={1}", ivp.Key, ivp.Value)).ToArray());

            if (tokenCustomSettings.IsNullOrTrimmedEmpty())
                tokenCustomSettings = null;

            return AddPrivilegeKey(isChannelGroupIdInsteadOfServerGroupId, groupId, channelId, tokenDescription, tokenCustomSettings);
        }

        /// <summary>
        /// Create a new token. If tokentype is set to 0, the ID specified with tokenid1 will be a server group ID. Otherwise,
        /// tokenid1 is used as a channel group ID and you need to provide a valid channel ID using tokenid2.
        /// </summary>
        /// <param name="isChannelGroupIdInsteadOfServerGroupId">if set to true, its a server group id</param>
        /// <param name="groupId">the id of the group</param>
        /// <param name="channelId">the id of the channel</param>
        /// <param name="tokenDescription">the description for the token</param>
        /// <param name="tokenCustomSettings">the custome settings you want to save with the token</param>
        public SingleValueResponse<string> AddPrivilegeKey(bool isChannelGroupIdInsteadOfServerGroupId, uint groupId, uint channelId, string tokenDescription, string tokenCustomSettings)
        {
            Command command = CommandName.PrivilegeKeyAdd.CreateCommand();
            command.AddParameter("tokentype", isChannelGroupIdInsteadOfServerGroupId);
            command.AddParameter("tokenid1", groupId);
            command.AddParameter("tokenid2", channelId);

            if (!tokenDescription.IsNullOrTrimmedEmpty())
                command.AddParameter("tokendescription", tokenDescription);

            if (!tokenCustomSettings.IsNullOrTrimmedEmpty())
                command.AddParameter("tokencustomset", tokenCustomSettings);

            return ResponseBase<SingleValueResponse<string>>.Parse(SendCommand(command), "token");
        }

        /// <summary>
        /// Deletes an existing PrivilegeKey matching the PrivilegeKey specified with token.
        /// </summary>
        /// <param name="privilegeKey">The token to delete</param>
        public SimpleResponse DeletePrivilegeKey(string privilegeKey)
        {
            if (privilegeKey.IsNullOrTrimmedEmpty())
                throw new ArgumentException("token is null or trimmed empty", "privilegeKey");

            Command command = CommandName.PrivilegeKeyDelete.CreateCommand();
            command.AddParameter("token", privilegeKey);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Use a PrivilegeKey to gain access to a server or channel group. Please note that the server will automatically delete the PrivilegeKey after it has been used.
        /// </summary>
        /// <param name="privilegeKey">The token to use</param>
        public SimpleResponse UsePrivilegeKey(string privilegeKey)
        {
            if (privilegeKey.IsNullOrTrimmedEmpty())
                throw new ArgumentException("token is null or trimmed empty", "privilegeKey");

            Command command = CommandName.PrivilegeKeyUse.CreateCommand();
            command.AddParameter("token", privilegeKey);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of offline messages you've received. The output contains the senders unique identifier, the messages subject, etc.
        /// </summary>
        public ListResponse<MessageEntry> GetMessageList()
        {
            return ListResponse<MessageEntry>.Parse(SendCommand(CommandName.MessageList.CreateCommand()));
        }

        /// <summary>
        /// Sends an offline message to the client specified by cluid
        /// </summary>
        /// <param name="clientUniqueId">the unique id of the receiver</param>
        /// <param name="subject">the subject</param>
        /// <param name="message">the message</param>
        public SimpleResponse AddMessage(string clientUniqueId, string subject, string message)
        {
            if (subject.IsNullOrTrimmedEmpty())
                throw new ArgumentException("subject is null or trimmed empty", "subject");

            if (message.IsNullOrTrimmedEmpty())
                throw new ArgumentException("message is null or trimmed empty", "message");

            Command command = CommandName.MessageAdd.CreateCommand();
            command.AddParameter("cluid", clientUniqueId);
            command.AddParameter("subject", subject);
            command.AddParameter("message", message);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays an existing offline message with ID msgid from your inbox. Please note that this does not
        /// automatically set the flag_read property of the message.
        /// </summary>
        /// <param name="messageId">the message id</param>
        /// <returns></returns>
        public GetMessageResponse GetMessage(uint messageId)
        {
            Command command = CommandName.MessageGet.CreateCommand();
            command.AddParameter("msgid", messageId);

            return ResponseBase<GetMessageResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Updates the flag_read property of the offline message specified with msgid. If flag is set to 1, the message will be marked as read.
        /// </summary>
        /// <param name="messageId">the message id</param>
        /// <param name="setRead">set it read or not</param>
        public SimpleResponse UpdateMessageFlag(uint messageId, bool setRead)
        {
            Command command = CommandName.MessageUpdateFlag.CreateCommand();
            command.AddParameter("msgid", messageId);
            command.AddParameter("flag", setRead);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Deletes an existing offline message with ID msgid from your inbox.
        /// </summary>
        /// <param name="messageId">the message id</param>
        public SimpleResponse DeleteMessage(uint messageId)
        {
            Command command = CommandName.MessageDel.CreateCommand();
            command.AddParameter("msgid", messageId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Displays a list of complaints on the selected virtual server.
        /// </summary>
        public ListResponse<ComplainListEntry> GetComplainList()
        {
            return GetComplainList(null);
        }

        /// <summary>
        /// Displays a list of complaints on the selected virtual server. If targetClientDatabaseId is specified, only complaints about the targeted client will be shown.
        /// </summary>
        public ListResponse<ComplainListEntry> GetComplainList(uint? targetClientDatabaseId)
        {
            Command command = CommandName.ComplainList.CreateCommand();

            if (targetClientDatabaseId.HasValue)
                command.AddParameter("tcldbid", targetClientDatabaseId.Value);

            return ListResponse<ComplainListEntry>.Parse(SendCommand(command), ComplainListEntry.Parse);
        }

        /// <summary>
        /// Submits a complaint about the client with database ID tcldbid to the server.
        /// </summary>
        /// <param name="targetClientDatabaseId">target client database id</param>
        /// <param name="message">the complain message</param>
        public SimpleResponse AddComplaint(uint targetClientDatabaseId, string message)
        {
            Command command = CommandName.ComplainAdd.CreateCommand();
            command.AddParameter("tcldbid", targetClientDatabaseId);
            command.AddParameter("message", message);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Deletes the complaint about the client with ID tcldbid submitted by the client with ID fcldbid from the server.
        /// </summary>
        /// <param name="targetClientDatabaseId">target client database id</param>
        /// <param name="sourceClientDatabaseId">source client database id</param>
        /// <returns></returns>
        public SimpleResponse DeleteComplaint(uint targetClientDatabaseId, uint sourceClientDatabaseId)
        {
            Command command = CommandName.ComplainDel.CreateCommand();
            command.AddParameter("tcldbid", targetClientDatabaseId);
            command.AddParameter("fcldbid", sourceClientDatabaseId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Deletes all complaints about the client with database ID tcldbid from the server.
        /// </summary>
        /// <param name="targetClientDatabaseId">target client database id</param>
        public SimpleResponse DeleteAllComplaints(uint targetClientDatabaseId)
        {
            Command command = CommandName.ComplainDelAll.CreateCommand();
            command.AddParameter("tcldbid", targetClientDatabaseId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public ListResponse<uint> BanClient(uint clientId)
        {
            return BanClient(clientId, null, null);
        }

        public ListResponse<uint> BanClient(uint clientId, TimeSpan duration)
        {
            return BanClient(clientId, duration, null);
        }

        public ListResponse<uint> BanClient(uint clientId, string reason)
        {
            return BanClient(clientId, null, reason);
        }

        /// <summary>
        /// Bans the client specified with ID clid from the server. Please note that this will create two separate ban rules
        /// for the targeted clients IP address and his unique identifier.
        /// </summary>
        /// <param name="clientId">the client id to ban</param>
        /// <param name="duration">the ban duration</param>
        /// <param name="reason">the reason for ban</param>
        public ListResponse<uint> BanClient(uint clientId, TimeSpan? duration, string reason)
        {
            Command command = CommandName.BanClient.CreateCommand();
            command.AddParameter("clid", clientId);

            if (duration.HasValue)
                command.AddParameter("time", Convert.ToUInt32(Math.Floor(duration.Value.TotalSeconds)));

            if (reason != null)
                command.AddParameter("banreason", reason);

            return ResponseBase<ListResponse<uint>>.Parse(SendCommand(command), "banid");
        }

        /// <summary>
        /// Displays a list of active bans on the selected virtual server.
        /// </summary>
        public ListResponse<BanListEntry> GetBanList()
        {
            return ListResponse<BanListEntry>.Parse(SendCommand(CommandName.BanList.CreateCommand()), BanListEntry.Parse);
        }

        /// <summary>
        /// Adds a new ban rule on the selected virtual server. All parameters are optional but at least one of the following must be set: ip, name, or uid.
        /// </summary>
        /// <param name="ipPattern">the ip pattern</param>
        /// <param name="namePattern">the name pattern</param>
        /// <param name="clientUniqueId">the clients unique id</param>
        /// <param name="duration">the duration</param>
        /// <param name="banReason">te reason for the ban</param>
        public SingleValueResponse<uint?> AddBanRule(string ipPattern, string namePattern, string clientUniqueId, TimeSpan? duration, string banReason)
        {
            Command command = CommandName.BanAdd.CreateCommand();

            if (!ipPattern.IsNullOrTrimmedEmpty())
                command.AddParameter("ip", ipPattern);

            if (!namePattern.IsNullOrTrimmedEmpty())
                command.AddParameter("name", namePattern);

            if (!clientUniqueId.IsNullOrTrimmedEmpty())
                command.AddParameter("uid", clientUniqueId);

            if (duration.HasValue)
                command.AddParameter("time", Convert.ToUInt32(Math.Floor(duration.Value.TotalSeconds)));

            if (!banReason.IsNullOrTrimmedEmpty())
                command.AddParameter("banreason", banReason);

            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "banid");
        }

        /// <summary>
        /// Deletes the ban rule with ID banid from the server.
        /// </summary>
        /// <param name="banId">the ban id</param>
        /// <returns></returns>
        public SimpleResponse DeleteBan(uint banId)
        {
            Command command = CommandName.BanDel.CreateCommand();
            command.AddParameter("banid", banId);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Deletes all active ban rules from the server.
        /// </summary>
        public SimpleResponse DeleteAllBans()
        {
            return ResponseBase<SimpleResponse>.Parse(SendCommand(CommandName.BanDelAll.CreateCommand()));
        }

        public InitializeFileUploadResponse InitializeFileUpload(uint clientFileTransferId, string name, uint channelId, ulong size, bool overwrite, bool resume)
        {
            return InitializeFileUpload(clientFileTransferId, name, channelId, size, overwrite, resume, null);
        }

        /// <summary>
        /// Initializes a file transfer upload. clientftfid is an arbitrary ID to identify the file transfer on client-side.
        /// On success, the server generates a new ftkey which is required to start uploading the file through TeamSpeak 3's file transfer interface.
        /// </summary>
        /// <param name="clientFileTransferId">the id of the file transfer</param>
        /// <param name="name">the name of the file to upload</param>
        /// <param name="channelId">the id of the channel to upload the file to</param>
        /// <param name="size">the size of the file in bytes</param>
        /// <param name="overwrite">whether to overwrite an existing file</param>
        /// <param name="resume">whether to resume on an existing file</param>
        /// <param name="channelPassword">the optional channel password</param>
        public InitializeFileUploadResponse InitializeFileUpload(uint clientFileTransferId, string name, uint channelId, ulong size, bool overwrite, bool resume, string channelPassword)
        {
            Command command = CommandName.FtInitUpload.CreateCommand();
            command.AddParameter("clientftfid", clientFileTransferId);
            command.AddParameter("name", name);
            command.AddParameter("cid", channelId);
            command.AddParameter("size", size);
            command.AddParameter("overwrite", overwrite);
            command.AddParameter("resume", resume);
            command.AddParameter("cpw", channelPassword);

            return ResponseBase<InitializeFileUploadResponse>.Parse(SendCommand(command));
        }

        public InitializeFileDownloadResponse InitializeFileDownload(uint clientFileTransferId, string name, uint channelId, ulong seekPosition)
        {
            return InitializeFileDownload(clientFileTransferId, name, channelId, seekPosition, null);
        }

        /// <summary>
        /// Initializes a file transfer download. clientftfid is an arbitrary ID to identify the file transfer on client-side.
        /// On success, the server generates a new ftkey which is required to start downloading the file through
        /// TeamSpeak 3's file transfer interface.
        /// </summary>
        /// <param name="clientFileTransferId">the id of the file transfer</param>
        /// <param name="name">the name of the file to upload</param>
        /// <param name="channelId">the id of the channel to upload the file to</param>
        /// <param name="seekPosition">the position of the file where to start downloading</param>
        /// <param name="channelPassword">the optional channel password</param>
        public InitializeFileDownloadResponse InitializeFileDownload(uint clientFileTransferId, string name, uint channelId, ulong seekPosition, string channelPassword)
        {
            Command command = CommandName.FtInitDownload.CreateCommand();
            command.AddParameter("clientftfid", clientFileTransferId);
            command.AddParameter("name", name);
            command.AddParameter("cid", channelId);
            command.AddParameter("seekpos", seekPosition);
            command.AddParameter("cpw", channelPassword);

            return ResponseBase<InitializeFileDownloadResponse>.Parse(SendCommand(command));
        }


        /// <summary>
        /// Displays a list of running file transfers on the selected virtual server. The output contains the path to which a
        /// file is uploaded to, the current transfer rate in bytes per second, etc.
        /// </summary>
        public ListResponse<FileTransferListEntry> GetFileTransferList()
        {
            return ListResponse<FileTransferListEntry>.Parse(SendCommand(CommandName.FtList.CreateCommand()), FileTransferListEntry.Parse);
        }

        public ListResponse<FileTransferFileEntry> GetFileList(uint channelId, string filePath)
        {
            return GetFileList(channelId, filePath, null);
        }

        /// <summary>
        /// Displays a list of files and directories stored in the specified channels file repository.
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="channelPassword">the password</param>
        /// <param name="filePath">the filepath</param>
        public ListResponse<FileTransferFileEntry> GetFileList(uint channelId, string filePath, string channelPassword)
        {
            if (filePath.IsNullOrTrimmedEmpty())
                throw new ArgumentException("filePath is null or trimmed empty", "filePath");

            Command command = CommandName.FtGetFileList.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cpw", channelPassword);
            command.AddParameter("path", filePath);

            return ListResponse<FileTransferFileEntry>.Parse(SendCommand(command), FileTransferFileEntry.Parse);
        }

        public ListResponse<FileTransferFileEntry> GetFileInfo(uint channelId, string name)
        {
            return GetFileInfo(channelId, name, null);
        }

        /// <summary>
        /// Displays detailed information about the specified file stored in a channels file repository.
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="name">the name of the file</param>
        /// <param name="channelPassword">the channel password</param>
        public ListResponse<FileTransferFileEntry> GetFileInfo(uint channelId, string name, string channelPassword)
        {
            return GetFileInfo(channelId, new[] {name}, channelPassword);
        }

        public ListResponse<FileTransferFileEntry> GetFileInfo(uint channelId, IEnumerable<string> names)
        {
            return GetFileInfo(channelId, names, null);
        }

        /// <summary>
        /// Displays detailed information about one or more specified files stored in a channels file repository.
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="names">the names of the files</param>
        /// <param name="channelPassword">the channel password</param>
        public ListResponse<FileTransferFileEntry> GetFileInfo(uint channelId, IEnumerable<string> names, string channelPassword)
        {
            if (names == null)
                throw new ArgumentNullException("names");

            if (names.Count() == 0)
                throw new ArgumentException("names is empty", "names");

            Command command = CommandName.FtGetFileInfo.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cpw", channelPassword);

            uint index = 0;
            foreach (string name in names)
            {
                command.AddParameter("name", name, index);
                index++;
            }

            return ListResponse<FileTransferFileEntry>.Parse(SendCommand(command), FileTransferFileEntry.Parse);
        }

        public SimpleResponse StopFileTransfer(uint serverFileTransferId)
        {
            return StopFileTransfer(serverFileTransferId, false);
        }

        /// <summary>
        /// Stops the running file transfer with server-side ID serverftfid.
        /// </summary>
        /// <param name="serverFileTransferId">transfer id</param>
        /// <param name="deleteFile">set to true, to delete the file</param>
        public SimpleResponse StopFileTransfer(uint serverFileTransferId, bool deleteFile)
        {
            Command command = CommandName.FtStop.CreateCommand();
            command.AddParameter("serverftfid", serverFileTransferId);
            command.AddParameter("delete", deleteFile);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public SimpleResponse DeleteFile(uint channelId, string filePath)
        {
            return DeleteFile(channelId, filePath, null);
        }

        /// <summary>
        /// Deletes one or more files stored in a channels file repository
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="channelPassword">the channel password</param>
        /// <param name="filePath">The filepath</param>
        public SimpleResponse DeleteFile(uint channelId, string filePath, string channelPassword)
        {
            if (filePath.IsNullOrTrimmedEmpty())
                throw new ArgumentException("filePath is null or trimmed empty", "filePath");

            Command command = CommandName.FtDeleteFile.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cpw", channelPassword);
            command.AddParameter("name", filePath);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public SimpleResponse CreateDirectory(uint channelId, string directoryPath)
        {
            return CreateDirectory(channelId, directoryPath, null);
        }

        /// <summary>
        /// Creates new directory in a channels file repository
        /// </summary>
        /// <param name="channelId">the channel id</param>
        /// <param name="channelPassword">the channel password</param>
        /// <param name="directoryPath">The path for the new directory</param>
        public SimpleResponse CreateDirectory(uint channelId, string directoryPath, string channelPassword)
        {
            if (directoryPath.IsNullOrTrimmedEmpty())
                throw new ArgumentException("directoryPath is null or trimmed empty", "directoryPath");

            Command command = CommandName.FtCreateDir.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cpw", channelPassword);
            command.AddParameter("dirname", directoryPath);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        public SimpleResponse RenameFile(uint channelId, string oldName, string newName)
        {
            return RenameFile(channelId, oldName, newName, null);
        }

        public SimpleResponse RenameFile(uint channelId, string oldName, string newName, string channelPassword)
        {
            return RenameFile(channelId, oldName, newName, null, channelPassword, null);
        }

        /// <summary>
        /// Renames a file in a channels file repository. If the two parameters tcid and tcpw are specified, the file will be moved into another channels file repository
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="channelPassword"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="targetChannelId"></param>
        /// <param name="targetChannelPassword"></param>
        /// <returns></returns>
        public SimpleResponse RenameFile(uint channelId, string oldName, string newName, uint? targetChannelId, string channelPassword, string targetChannelPassword)
        {
            if (oldName.IsNullOrTrimmedEmpty())
                throw new ArgumentException("oldName is null or trimmed empty", "oldName");

            if (newName.IsNullOrTrimmedEmpty())
                throw new ArgumentException("oldName is null or trimmed empty", "newName");

            Command command = CommandName.FtRenameFile.CreateCommand();
            command.AddParameter("cid", channelId);
            command.AddParameter("cpw", channelPassword);
            command.AddParameter("oldname", oldName);
            command.AddParameter("newname", newName);

            if (targetChannelId.HasValue)
                command.AddParameter("tcid", targetChannelId.Value);

            if (targetChannelPassword != null)
                command.AddParameter("tcpw", targetChannelPassword);

            return ResponseBase<SimpleResponse>.Parse(SendCommand(command));
        }

        /// <summary>
        /// Searches for custom client properties specified by ident and value. The value parameter can include regular characters and SQL wildcard characters (e.g. %).
        /// </summary>
        /// <param name="ident">the ident</param>
        /// <param name="pattern">the pattern used for searching</param>
        public ListResponse<CustomSearchEntry> CustomSearch(string ident, string pattern)
        {
            Command command = CommandName.CustomSearch.CreateCommand();
            command.AddParameter("ident", ident);
            command.AddParameter("pattern", pattern);

            return ListResponse<CustomSearchEntry>.Parse(SendCommand(command), CustomSearchEntry.Parse);
        }

        /// <summary>
        /// Displays a list of custom properties for the client specified with cldbid.
        /// </summary>
        /// <param name="clientDatabaseId">the clients database id</param>
        public ListResponse<CustomInfoEntry> GetCustomInfo(int clientDatabaseId)
        {
            Command command = CommandName.CustomInfo.CreateCommand();
            command.AddParameter("cldbid", clientDatabaseId);

            return ListResponse<CustomInfoEntry>.Parse(SendCommand(command), CustomInfoEntry.Parse);
        }

        /// <summary>
        /// Displays information about your current ServerQuery connection including the ID of the selected virtual server, your loginname, etc.
        /// </summary>
        public WhoAmIResponse SendWhoAmI()
        {
            return ResponseBase<WhoAmIResponse>.Parse(SendCommand(SharedCommandName.WhoAmI.CreateCommand()));
        }

        /// <summary>
        /// Creates a copy of the server group specified with sourceGroupId.
        /// </summary>
        /// <param name="sourceGroupId">The id of the source group</param>
        /// <param name="targetGroupName">The name of the new group</param>
        /// <param name="groupType">The type of the group to create</param>
        public SingleValueResponse<uint?> CopyServerGroup(uint sourceGroupId, string targetGroupName, GroupDatabaseType groupType)
        {
            Command command = CommandName.ServerGroupCopy.CreateCommand();
            command.AddParameter("ssgid", sourceGroupId);
            command.AddParameter("tsgid", 0);
            command.AddParameter("name", targetGroupName);
            command.AddParameter("type", (int) groupType);

            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "sgid");
        }

        /// <summary>
        /// Overwrites the settings of a server group with the settings of another group
        /// </summary>
        /// <param name="sourceGroupId">The id of the source group from which to copy settings</param>
        /// <param name="targetGroupId">The id of the group to overwrite</param>
        /// <param name="groupType">The type of the group to create</param>
        public SingleValueResponse<uint?> OverwriteServerGroup(uint sourceGroupId, int targetGroupId, GroupDatabaseType groupType)
        {
            Command command = CommandName.ServerGroupCopy.CreateCommand();
            command.AddParameter("ssgid", sourceGroupId);
            command.AddParameter("tsgid", targetGroupId);
            command.AddParameter("name", "-");
            command.AddParameter("type", (int)groupType);

            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "sgid");
        }

        /// <summary>
        /// Creates a copy of the channel group specified with sourceGroupId.
        /// </summary>
        /// <param name="sourceGroupId">The id of the source group</param>
        /// <param name="targetGroupName">The name of the new group</param>
        /// <param name="groupType">The type of the group to create</param>
        public SingleValueResponse<uint?> CopyChannelGroup(uint sourceGroupId, string targetGroupName, GroupDatabaseType groupType)
        {
            Command command = CommandName.ChannelGroupCopy.CreateCommand();
            command.AddParameter("scgid", sourceGroupId);
            command.AddParameter("tcgid", 0);
            command.AddParameter("name", targetGroupName);
            command.AddParameter("type", (int)groupType);

            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "sgid");
        }

        /// <summary>
        /// Overwrites the settings of a channel group with the settings of another group
        /// </summary>
        /// <param name="sourceGroupId">The id of the source group from which to copy settings</param>
        /// <param name="targetGroupId">The id of the group to overwrite</param>
        /// <param name="groupType">The type of the group to create</param>
        public SingleValueResponse<uint?> OverwriteChannelGroup(uint sourceGroupId, int targetGroupId, GroupDatabaseType groupType)
        {
            Command command = CommandName.ChannelGroupCopy.CreateCommand();
            command.AddParameter("scgid", sourceGroupId);
            command.AddParameter("tcgid", targetGroupId);
            command.AddParameter("name", "-");
            command.AddParameter("type", (int)groupType);

            return ResponseBase<SingleValueResponse<uint?>>.Parse(SendCommand(command), "cgid");
        }

        #endregion


    }
}
