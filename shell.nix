{ pkgs ? import <nixpkgs> {} }: pkgs.mkShell {
  nativeBuildInputs = with pkgs; [
    (python3.withPackages (py: [
      py.flask
      py.requests
    ]))
  ];
}
