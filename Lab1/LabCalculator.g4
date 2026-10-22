// Grammar for Variant 63
grammar LabCalculator;

compileUnit : expression EOF;

// Syntax rules for parser
expression
// Highest priority
	: LPAREN expression RPAREN #ParenthesizedExpr
	| operatorToken=MAX LPAREN left=expression COMMA right=expression RPAREN #MaxExpr
	| operatorToken=MIN LPAREN left=expression COMMA right=expression RPAREN #MinExpr
	| NUMBER #NumExpr
	| CELL   #CellExpr
// Arythmetic operations
	| left=expression operatorToken=(MOD | DIV) right=expression #ArithmExpr
// Comparison operations
	| left=expression operatorToken=(LE | GE | NE) right=expression #CompareExpr
// Logic not
	| operatorToken=NOT expression #NotExpr
// Logic and
	| left=expression operatorToken=AND right=expression #AndExpr
// Logic or
	| left=expression operatorToken=OR right=expression #OrExpr;

// Grammar rules
// 2
MOD : 'mod' | 'MOD';
DIV : 'div' | 'DIV';
// 6
MAX : 'max' | 'MAX';
MIN : 'min' | 'MIN';
// 9 
LE : '<=';
GE : '>=';
NE : '<>';
// 10
NOT : 'not' | 'NOT';
// 11
OR : 'or' | 'OR';
AND : 'and' | 'AND';

// Basic rules
LPAREN : '(';
RPAREN : ')';
COMMA : ',';
NUMBER : [0-9]+;
WS : [ \t\r\n]+ -> channel(HIDDEN);
CELL : [a-zA-Z]+[1-9][0-9]*;




