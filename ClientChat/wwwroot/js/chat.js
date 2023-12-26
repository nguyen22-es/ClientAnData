$(document).ready(function () {
    function GetsToken() {
        return $.ajax({
            url: 'Token/GetsToken',
            method: 'GET',
            dataType: 'json'
        });
    }



    var connection = new signalR.HubConnectionBuilder()
        .withUrl('https://localhost:7191/SignalrHub', {
            accessTokenFactory: () => GetsToken()
        })
        .build();

    connection.start().then(function () {
        console.log('SignalR Started...');
    }).catch(function (err) {
        console.log('Không thể kết nối SignalR...');
        return console.error(err);
    });
 
    connection.on("getProfileInfo", function (name, avatar, id) {
        viewModel.myName(name);
       viewModel.myAvatar(avatar);
       viewModel.idUser(id)
    });
   /* function ChatMessage(content, timestamp, nameSend, userID, chatUSerID, roomID, room, avatar, isMine) {
        var self = this;
        self.content = ko.observable(content);
        self.timestamp = ko.observable(timestamp);
        self.nameSend = ko.observable(nameSend);
        self.userID = ko.observable(userID);
        self.roomID = ko.observable(roomID);
        self.chatUSerID = ko.observable(chatUSerID);
        self.room = ko.observable(room);
        self.avatar = ko.observable(avatar);
        self.isMine = ko.observable(isMine);
    }*/
    connection.on("AddUser", function (user) {
       alert(user + "đã được thêm vào nhóm chat")
    });
    connection.on("newMessage", function (messageView) {
        var isMine = messageView.userID === viewModel.idUser();
        var message = new ChatMessage(messageView.content, messageView.timestamp, messageView.nameSend, messageView.chatUSerID, messageView.roomID, messageView.room, messageView.userID, messageView.avatar, isMine);
        viewModel.chatMessages.push(message);
        $(".chat-body").animate({ scrollTop: $(".chat-body")[0].scrollHeight }, 1000);
    });

    function AppViewModel()
    {

        var self = this;
        self.chatRooms = ko.observableArray([]);
        self.Users = ko.observableArray([]);
        self.ChatUsers = ko.observableArray([]);
        self.chatMessages = ko.observableArray([]);
        self.myName = ko.observable('');
        self.myAvatar = ko.observable('');
        self.idUser = ko.observable('');
        self.message = ko.observable("");
        self.RoomIndexID = ko.observable("");
        self.isChatRoom = ko.observable(false);
        self.createRoom = ko.observable('');
        self.onEnter = function (d, e) {
            if (e.keyCode === 13) {
                self.sendNewMessage();
            }
            return true;
        }
        self.sendNewMessage = function () {
         
         
            if (self.isChatRoom()) {
                self.sendToRoom(self.message());
               
              
            }
            else {
                self.sendChatUser(self.message());
            }

            self.message("");
        }

        /// tạo room mới

        self.Create = function () {
            $.ajax({
                type: "GET",
                url: "ChatRoom/CreateRoom",
                dataType: 'json',
                data: { NameRoom: self.createRoom() },
                success: function (data) {                                  
                    self.GetRooms()                
                },
                error: function (error) {
                    console("Có lỗi xảy ra: " + error.responseText);
                }
            });
        }




        // thông tin nhưng phòng mình tham gia
        self.GetRooms = function ()
        {
            $.ajax({
                type: "GET",
                url: "ChatRoom/GetRooms",
                dataType: 'json',
                success: function (data) {
                    self.chatRooms.removeAll();
                    for (var i = 0; i < data.length; i++) {
                        var isAdmin = data[i].admin == self.idUser();
                        self.chatRooms.push(new ChatRoom(data[i].roomID, data[i].nameRoom, data[i].admin, isAdmin));                     
                    }
                },
                error: function (error) {
                    console("Có lỗi xảy ra: " + error.responseText);
                }
            });

        }
        // những tin nhắn trong phòng 
        self.getMessages = function (room)
        {           
            $.ajax({
                method: 'GET',
                url: 'Message/GetMessageRoom',

                dataType: 'json',
                data: { RoomID: room },
                success: function (data) {
                    self.isChatRoom(true); 
                    self.RoomIndexID = room
                    self.chatMessages.removeAll();
                    for (var i = data.length - 1; i => 0; i--) {
                        var isMine = data[i].nameSend == self.myName();
                        self.chatMessages.push(new ChatMessage(
                            data[i].content,
                            data[i].timestamp,
                            data[i].nameSend,
                            data[i].userID,
                            data[i].chatUSerID,
                            data[i].roomID,
                            data[i].room,
                            data[i].avatar,
                            isMine
                        ));
                      
                    }

                       $(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);
                },
                error: function (error) {
                    console.error(error);
                }
            });
        }
        // tạo tin nhắn trong phòng
        self.sendToRoom  =   function (message) {
            var formData = new FormData();
            formData.append("message", message);
            formData.append("RoomID", self.RoomIndexID());

            $.ajax({
                type: "POST",
                url: "/Message/CreateMessageRoom",
                contentType: false,
                processData: false,
                data: formData,
                success: function () {                 
                },
                error: function (error) {
                    console("Có lỗi xảy ra: " + error.responseText);
                }
            });

        }

        // những tin nhắn riêng

        self.GetChatUsers = function (chatUserID) {
            $.ajax({
                method: 'GET',
                url: 'Message/GetChatUser',

                dataType: 'json',
                data: { id: chatUserID },
                success: function (data) {
                    self.isChatRoom(false);
                    self.RoomIndexID = chatUserID();
                    self.chatMessages.removeAll();
                    for (var i = data.length - 1; i => 0; i--) {
                        var isMine = data[i].nameSend == self.myName();
                        self.chatMessages.push(new ChatMessage(
                            data[i].content,
                            data[i].timestamp,
                            data[i].nameSend,
                            data[i].userID,
                            data[i].chatUSerID,
                            data[i].roomID,
                            data[i].room,
                            data[i].avatar,
                            isMine
                        ));                                             
                    }

                    $(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);
                },
                error: function (error) {
                    console.error(error);
                }
            });
        }


        // GetChatUser
            self.GetChatUser = function () {

                $.ajax({
                    url: "/ChatRoom/GetChatUsers",
                    type: "GET",
                    dataType: 'json',
                    success: function (data) {
                        self.ChatUsers.removeAll();
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].friendChatID != self.idUser()) {
                                self.ChatUsers.push(new ChatUser(data[i].chatUserID, data[i].friendChatID, data[i].friendChatName));
                            }
                        }
                    },
                    error: function (error) {
                        console("Có lỗi xảy ra: " + error.responseText);
                    }
                });
            }
        self.GetChatOther = function (ID) {

            $.ajax({
                method: 'GET',
                url: 'ChatRoom/CreateChatUser',

                dataType: 'json',
                data: { FriendID: ID },
                success: function (data) {
                    self.isChatRoom(false);
                    self.chatMessages.removeAll();
                    for (var i = data.length - 1; i => 0; i--) {
                        var isMine = data[i].nameSend == self.myName();
                        self.chatMessages.push(new ChatMessage(
                            data[i].content,
                            data[i].timestamp,
                            data[i].nameSend,
                            data[i].userID,
                            data[i].chatUSerID,
                            data[i].roomID,
                            data[i].room,
                            data[i].avatar,
                            isMine
                        ));
                        self.RoomIndexID = data[0].chatUSerID

                    }

                    $(".messages-container").animate({ scrollTop: $(".messages-container")[0].scrollHeight }, 1000);
                },
                error: function (error) {
                    console.error(error);
                }
            });
        }
        
        // tạo tin nhắn riêng 
        self.sendChatUser = function (message) {
            var formData = new FormData();
            formData.append("message", message);
            formData.append("chatUSerID", self.RoomIndexID);

            $.ajax({
                type: "POST",
                url: "/Message/CreateMessageUser",
                contentType: false,
                processData: false,
                data: formData,
                success: function () {
                },
                error: function (error) {
                    console("Có lỗi xảy ra: " + error.responseText);
                }
            });

        }


        /// get All User-----///

        self.GetAllUserr = function () {
          
            $.ajax({
                url: "/Home/GetUser",
                type: "GET",
                dataType: 'json',
                data: {roomID: self.RoomIndexID},
                success: function (data) {
                    self.Users.removeAll();
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].friendChatID != self.idUser()) {
                            self.Users.push(new User(data[i].id, data[i].displayName, data[i].avartar));
                        }
                    }
                },
                error: function (error) {
                    console("Có lỗi xảy ra: " + error.responseText);
                }
            });
        }
        //--------------------///

        //---------------------///
        //thêm user
        self.joinRoom = function (UserID) {
            var formData = new FormData();
            formData.append("RoomID", self.RoomIndexID());
            formData.append("userID", UserID());

            $.ajax({
                type: "POST",
                url: "/ChatRoom/AddUser",
                contentType: false,
                processData: false,
                data: formData,
                success: function () {
                },
                error: function (error) {
                    console("Có lỗi xảy ra: " + error.responseText);
                }
            });

        }

        self.uploadFiles = function () {
            var form = document.getElementById("uploadForm");
            $.ajax({
                type: "POST",
                url: '/api/Upload',
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function () {
                    $("#UploadedFile").val("");
                },
                error: function (error) {
                    alert('Error: ' + error.responseText);
                }
            });
        }


    }

    var viewModel = new AppViewModel();
    ko.applyBindings(viewModel);
    function ChatRoom(roomID, nameRoom, admin,isAdmin) {
            var self = this;
        self.roomID = ko.observable(roomID);
        self.nameRoom = ko.observable(nameRoom);
        self.admin = ko.observable(admin);
        self.isAdmin = ko.observable(isAdmin);
        }



    function ChatUser(chatUserID, friendChatID, friendChatName) {
        var self = this;
        self.chatUserID = ko.observable(chatUserID);
        self.friendChatID = ko.observable(friendChatID);
        self.friendChatName = ko.observable(friendChatName);
    }
    function User(UserID, UserName, UserAvatar) {
        var self = this;
        self.UserID = ko.observable(UserID);
        self.UserName = ko.observable(UserName);
        self.UserAvatar = ko.observable(UserAvatar);
    }

    function ChatMessage(content, timestamp, nameSend, userID, chatUSerID, roomID, room, avatar, isMine) {
        var self = this;
        self.content = ko.observable(content);
        self.timestamp = ko.observable(timestamp);
        self.nameSend = ko.observable(nameSend);
        self.userID = ko.observable(userID);
        self.roomID = ko.observable(roomID);

        self.chatUSerID = ko.observable(chatUSerID);

        self.room = ko.observable(room);
        self.avatar = ko.observable(avatar);
        self.isMine = ko.observable(isMine);
    }
    






















































 
   
});