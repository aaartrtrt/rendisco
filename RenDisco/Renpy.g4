grammar Renpy;

tokens {
  INDENT,
  DEDENT,
  WITH,
  FADEIN,
  FADEOUT,
  JUMP,
  CALL,
  RETURN,
  MENU,
  DEFAULT,
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
  statement (NL* statement)*;

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
  | assignment
  | dialogue
  | narration
  | conditional_block
  ;

label_def:
  'label' IDENT (aguments)? ':' (INDENT block DEDENT)?
  ;

character_def:
  'define' IDENT '=' 'Character' '(' STRING ',' ( 'color' '=' STRING ')' )?
  ;

scene_def:
  'scene' 'bg' IDENT ('with' (
    'dissolve'
    | 'fade'))? //TODO: Implement transitions
  ;

pause_def:
  'pause' NUMBER
  ;

play_music_def:
  'play music' STRING ('fadein' NUMBER)?
  ;

stop_music_def:
  'stop music' ('fadeout' NUMBER)?
  ;

jump_def:
  'jump' IDENT (aguments)?
  ;

call_def:
  'call' IDENT (aguments)?
  ;

menu_def:
  'menu' ':'
  INDENT
  (menu_option)*
  DEDENT
  ;

menu_option:
  STRING ':' INDENT block DEDENT
  ;

default_def:
  'default' IDENT '=' expression
  ;

return_def:
  'return'
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

aguments:
  '(' argument (',' argument)* ')'
  ;

argument:
  (IDENT '=')? expression   // Optional parameter hNUMBERing
  ;

conditional_block:
  'if' expression ':' INDENT block DEDENT
  (elif_block)*
  (else_block)?
  ;

elif_block:
  'elif' expression ':' INDENT block DEDENT
  ;

else_block:
  'else' ':' INDENT block DEDENT
  ;

assignment:
  '$' IDENT '=' expression
  ;

expression:
  NUMBER
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

NUMBER: 
  [0-9]+ ('.' [0-9]+)? 
  | '.' [0-9]+ 
  | [0-9]+
  ;

NL:
  (' '* '\r'? '\n' ' '*); // Note the ' '*

WS: [ \t]+ -> skip;

LINE_COMMENT: '#' ~[\r\n]* -> skip;
