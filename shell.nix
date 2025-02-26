{ pkgs ? import <nixpkgs> {} }: pkgs.mkShell {
  nativeBuildInputs = with pkgs; [
    pipreqs
    (python3.withPackages (py: [
      py.flask
      py.requests
      py.pymongo
      py.python-dotenv
    ]))
  ];
}
