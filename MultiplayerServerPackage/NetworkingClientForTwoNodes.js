const NetworkingBehaviour = require("./NetworkingBehaviour");

new NetworkingBehaviour(Main);

function Main(program) {
    program.io.Connect((server) => {
        program.Update(() => {

        });
        server.emit("hello", "hello");
        server.on("hello", () => {
            program.print("tsedfagd");
        });
    });
}