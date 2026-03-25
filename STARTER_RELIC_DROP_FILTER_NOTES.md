This patch restores CylinderRelic to the Gunslinger relic registry so the character can be selected and initialized normally,
while excluding it from the unlocked/random relic pool by filtering it out in GetUnlockedRelics(...).

Expected behavior after applying:
- Gunslinger can be selected normally.
- CylinderRelic is still granted as the starting relic.
- CylinderRelic should not appear in normal relic rewards generated from the Gunslinger relic pool.
