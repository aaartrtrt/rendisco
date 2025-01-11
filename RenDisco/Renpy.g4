grammar Renpy;

tokens {
  INDENT,
  DEDENT,
  LABEL,
  DEFINE,
  CHARACTER,
  COLOR,
  SCENE,
  WITH,
  PAUSE,
  PLAY,
  MUSIC,
  FADEIN,
  FADEOUT,
  JUMP,
  CALL,
  RETURN,
  MENU,
  DEFAULT,
  STOP,
  IF,
  ELIF,
  ELSE,
  TRUE,
  FALSE
}

@lexer::header {
  using AntlrDenter;
}

@lexer::members {
  private DenterHelper denter;

  public override IToken NextToken()
  {
    if (denter == null)
    {
      denter = DenterHelper.Builder()
        .Nl(NL)
        .Indent(RenpyParser.INDENT)
        .Dedent(RenpyParser.DEDENT)
        .PullToken(base.NextToken);
    }

    return denter.NextToken();
  }
}

block:
  statement NL (block)?;

statement:
  label_def
  | character_def
  | scene_def
  | pause_def
  | play_music_def
  | stop_music_def
  | jump_def
  | call_def
  | menu_def
  | default_def
  | return_def
  | dialogue
  | narration
  | conditional_block
  ;

label_def:
  LABEL IDENT (argument)? ':' (INDENT block DEDENT)?
  ;

character_def:
  DEFINE IDENT '=' CHARACTER '(' STRING ')' (COLOR '(' STRING ')' )?
  ;

scene_def:
  SCENE 'bg' IDENT // (WITH dissolve)? TODO: Implement transitions
  ;

pause_def:
  PAUSE INT
  ;

play_music_def:
  PLAY MUSIC STRING (FADEIN FLOAT)?
  ;

stop_music_def:
  STOP MUSIC (FADEOUT FLOAT)?
  ;

jump_def:
  JUMP IDENT (argument)?
  ;

call_def:
  CALL IDENT (argument)?
  ;

menu_def:
  MENU ':' NL INDENT (menu_option)* DEDENT
  ;

menu_option:
  STRING ':' NL INDENT block DEDENT
  ;

default_def:
  DEFAULT IDENT '=' expression
  ;

return_def:
  RETURN
  ;

dialogue:
  character_ref STRING
  ;

narration:
  STRING
  ;

character_ref:
  IDENT
  ;

argument:
  '(' expression ')' 
  ;

conditional_block:
  IF expression ':' INDENT block DEDENT
  (ELIF expression ':' INDENT block DEDENT)*
  (ELSE ':' INDENT block DEDENT)?
  ;

expression:
  INT
  | FLOAT
  | STRING
  | IDENT
  | expression ('+'|'-'|'*'|'/') expression 
  | expression '<' expression
  | expression '>' expression
  | expression '=' expression 
  | TRUE 
  | FALSE 
  | '(' expression ')'
  ;

IDENT: [a-zA-Z_][a-zA-Z0-9_]*;

STRING: '"' .*? '"'; // Simple string matching (improve for escaping)

FLOAT: 
  [0-9]+ ('.' [0-9]+)? 
  | '.' [0-9]+ 
  ;

INT: [0-9]+;

NL: ('\r'? '\n' ' '*); // Note the ' '*

WS: [ \t]+ -> skip;

LINE_COMMENT: '#' ~[\r\n]* -> skip;