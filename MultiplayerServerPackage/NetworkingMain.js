const NetworkingBehaviour = require("./NetworkingBehaviour");

new NetworkingBehaviour(Main);

function Main(program) {
    program.io.OnConnection(client => {
        program.setEvents(client, {
            hello: "emit->others"
        });
    });
}