class NetworkingBehaviour
{
    constructor(main)
    {
        var si = setInterval(() => {}, 1000);
        
        var program = {
            print: function(text) {
                console.log(text);
            },
            exit: function()
            {
                ClearInterval(si);
            },
            Update: function(callback) {
                setInterval(callback, 1000);
            },
            io: {
                OnConnection: function(callback) {
                    var app = require('express')();
                    var http = require('http').Server(app);
                    var io = require('socket.io')(http);
            
                    http.listen(3000, function () {
                    console.log('listening on *:3000');
                    });
                    io.on('connection', callback);
                },
                Connect: function(callback) {
                    var io = require('socket.io-client');
                    var socket = io.connect('http://localhost:3000', {reconnect: true});
                    // Add a connect listener
                    socket.on('connect', function (data) {
                        callback(socket);
                    });
                }
            },
            setEvents: function(socket,obj)
            {
                for (const el in obj) {
                    if (Object.hasOwnProperty.call(obj, el)) {
                        const element = obj[el];
                        switch (typeof(element)) {
                            case "string":
                                socket.on(el, data => {
                                    switch (element) {
                                        case "emit->sender":
                                            socket.emit(el, data);
                                            break;
                                        case "emit->all":
                                            socket.emit(el, data);
                                            socket.broadcast.emit(el, data);
                                            break;
                                            case "emit->others":
                                                console.log("emitted to others");
                                                socket.broadcast.emit(el, data);
                                                break;
                                        default:
                                            break;
                                    }
                                });
                                break;
                            default:
                                socket.on(el, element);
                                break;
                        }
                    }
                }
            }
        };
        main(program);
    }
}
module.exports = NetworkingBehaviour;