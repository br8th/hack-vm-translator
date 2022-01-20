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

// push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 16
@16
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
@TRUE_1
D;JEQ
(FALSE_1)
@SP
A=M-1
M=0
@END_1
0;JMP
(TRUE_1)
@SP
A=M-1
M=-1
(END_1)

// push constant 16
@16
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
@TRUE_2
D;JEQ
(FALSE_2)
@SP
A=M-1
M=0
@END_2
0;JMP
(TRUE_2)
@SP
A=M-1
M=-1
(END_2)

// push constant 892
@892
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 891
@891
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
@FALSE_3
D;JGT
@FALSE_3
D;JEQ
(TRUE_3)
@SP
A=M-1
M=-1
@END_3
0;JMP
(FALSE_3)
@SP
A=M-1
M=0
(END_3)

// push constant 891
@891
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 892
@892
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
@FALSE_4
D;JGT
@FALSE_4
D;JEQ
(TRUE_4)
@SP
A=M-1
M=-1
@END_4
0;JMP
(FALSE_4)
@SP
A=M-1
M=0
(END_4)

// push constant 891
@891
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 891
@891
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
@FALSE_5
D;JGT
@FALSE_5
D;JEQ
(TRUE_5)
@SP
A=M-1
M=-1
@END_5
0;JMP
(FALSE_5)
@SP
A=M-1
M=0
(END_5)

// push constant 32767
@32767
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 32766
@32766
D=A
@SP
A=M
M=D
@SP
M=M+1

// gt
@SP
M=M-1
A=M
D=M
A=A-1
D=D-M
@FALSE_6
D;JGT
@FALSE_6
D;JEQ
(TRUE_6)
@SP
A=M-1
M=-1
@END_6
0;JMP
(FALSE_6)
@SP
A=M-1
M=0
(END_6)

// push constant 32766
@32766
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 32767
@32767
D=A
@SP
A=M
M=D
@SP
M=M+1

// gt
@SP
M=M-1
A=M
D=M
A=A-1
D=D-M
@FALSE_7
D;JGT
@FALSE_7
D;JEQ
(TRUE_7)
@SP
A=M-1
M=-1
@END_7
0;JMP
(FALSE_7)
@SP
A=M-1
M=0
(END_7)

// push constant 32766
@32766
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 32766
@32766
D=A
@SP
A=M
M=D
@SP
M=M+1

// gt
@SP
M=M-1
A=M
D=M
A=A-1
D=D-M
@FALSE_8
D;JGT
@FALSE_8
D;JEQ
(TRUE_8)
@SP
A=M-1
M=-1
@END_8
0;JMP
(FALSE_8)
@SP
A=M-1
M=0
(END_8)

// push constant 57
@57
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 31
@31
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 53
@53
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

// push constant 112
@112
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

// neg
@SP
A=M-1
M=-M

// and
@SP
A=M-1
D=M
A=A-1
M=M&D
@SP
M=M-1

// push constant 82
@82
D=A
@SP
A=M
M=D
@SP
M=M+1

// or
@SP
A=M-1
D=M
A=A-1
M=M|D
@SP
M=M-1

// not
@SP
A=M-1
M=!M

