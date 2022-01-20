// push constant 7
@7
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 8
@8
D=A
@SP
A=M
M=D
@SP
M=M+1

// add
@SP
A=M-1
D=M
A=A-1
M=M+D
@SP
M=M-1

// push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// eq
@SP
M=M-1
A=M
D=M
A=A-1
D=D-M
@TRUE_0
D;JEQ
(FALSE_0)
@SP
A=M-1
M=0
@END_0
0;JMP
(TRUE_0)
@SP
A=M-1
M=-1
(END_0)

// push constant 1
@1
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 2
@2
D=A
@SP
A=M
M=D
@SP
M=M+1

// neg
@SP
A=M-1
M=-M

// push constant 7
@7
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 8
@8
D=A
@SP
A=M
M=D
@SP
M=M+1

// sub
@SP
A=M-1
D=M
A=A-1
M=M-D
@SP
M=M-1

// push constant 57
@57
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 28
@28
D=A
@SP
A=M
M=D
@SP
M=M+1

// and
@SP
A=M-1
D=M
A=A-1
M=M&D
@SP
M=M-1

// push constant 2
@2
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 1
@1
D=A
@SP
A=M
M=D
@SP
M=M+1

// lt
@SP
M=M-1
A=M
D=M
A=A-1
D=M-D
@FALSE_1
D;JGT
@FALSE_1
D;JEQ
(TRUE_1)
@SP
A=M-1
M=-1
@END_1
0;JMP
(FALSE_1)
@SP
A=M-1
M=0
(END_1)

