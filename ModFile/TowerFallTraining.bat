@echo off
:loop
python python\_run_towerfall_for_training.py
echo Le script Python s'est terminé. Redémarrage...
goto loop