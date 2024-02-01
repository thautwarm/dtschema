function rename_type(x) {
  if (x == "str") {
    return "string";
  }
  if (x == "token") {
    return "IToken";
  }
  if (["int", "float", "bool"].includes(x)) {
    return x;
  }
  if ("list" == x) {
    return "System.Collections.Generic.List";
  }
  return x;
}

function rename_var(x) {
  return x;
}

function rename_field(x) {
  return x;
}

module.exports = { rename_type, rename_var, rename_field };
