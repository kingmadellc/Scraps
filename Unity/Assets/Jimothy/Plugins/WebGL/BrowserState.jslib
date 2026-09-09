mergeInto(LibraryManager.library, {
  JimothyPublishState: function(json) {
    window.jimothyState = UTF8ToString(json);
    window.render_game_to_text = function() { return window.jimothyState; };
  }
});
