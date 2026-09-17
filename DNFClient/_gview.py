# -*- coding: utf-8 -*-
"""打印乱码行按 GBK 字节看待的"残缺明文"（□=已丢失字节，其余=可辨字符）。"""
import os
import sys

FILES = [
    r'Assets\Scripts\SkillSystem\Runtime\Logic\SkillDamage.cs',
    r'Assets\Scripts\LogicLayer\LogicActorSkill.cs',
    r'Assets\Scripts\SkillSystem\BuffSystem\Buff\RepelBuff.cs',
]
BASE = r'f:\My_Project\Test-Project\DNFClient'


def partial(line):
    out, buf = [], bytearray()

    def flush():
        if buf:
            out.append(buf.decode('gbk', 'replace'))
            buf.clear()

    for ch in line:
        if ch == '\ufffd':
            flush()
            out.append('□')
        else:
            buf.extend(ch.encode('utf-8'))
    flush()
    return ''.join(out)


for f in FILES:
    p = os.path.join(BASE, f)
    lines = open(p, 'rb').read().decode('utf-8').split('\r\n')
    print('=' * 25, os.path.basename(f), '=' * 25)
    for i, l in enumerate(lines, 1):
        if '\ufffd' in l:
            lead = l[:len(l) - len(l.lstrip())]
            print('%3d|%s%s' % (i, lead, partial(l.strip())))
    print()
