using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class VillageControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WheatText;//����� �������
    [SerializeField] TextMeshProUGUI PeasantText;//����� ��������
    [SerializeField] TextMeshProUGUI WarText;//����� ������
    [SerializeField] TextMeshProUGUI QuantiAttackText;//���-�� ����������
    [SerializeField] TextMeshProUGUI VolumeAudioButtonText;//����� ������ ��������� �����


    [SerializeField] Image WheatTimerImage;//����������� ������� ������ ����� ������
    [SerializeField] Image EatingTimerImage;//����������� ������� ������ ����
    [SerializeField] Image AttackTimerImage;//����������� ������� ���������
    [SerializeField] Image PeasantTimerImage;//����������� ������� ��������� ������ �����������
    [SerializeField] Image WarTimerImage;//����������� ������� ��������� ������ �����

    [SerializeField] Button PeasantButton;//������ ������ �����������
    [SerializeField] Button WarButton;//������ ������ �����

    [SerializeField] GameObject endGamePanel;//������ ���������
    [SerializeField] GameObject winGamePanel;//������ ������
    [SerializeField] GameObject PausePanel;//������ �����

    [SerializeField] TextMeshProUGUI[] FinalStatisticsText = new TextMeshProUGUI[4];//����� �� ������ ��� ����� �������� ����������
    [SerializeField] AudioSource[] GameAudioSources = new AudioSource[3];  

    int peasant = 5, 
        war = 0,
        wheat = 15; //���������� �������� �������
    int totalPeople;//����� �����
    int attack = 0;//���������� �����
    int nullAttacker = 0;//��������� ��� ����� ��� ������


    float getWheatTime, //����� ��� ��������� �������
          getPeasantTime,//����� ��� ��������� ������ �����������
          getWarTime,//����� ��� ��������� ������ �����
          eatingTime,//����� ������ ����
          attackTime,//����� ��������� 
          attackTimeIncrease = 25;//����� ��� ��������� 

    bool hirePeasant = false;//��������� ��� ������������ ������� ������ ��� �� ������ �����������
    bool hireWar = false;//��������� ��� ������������ ������� ������ ��� �� ������ �����
    bool endGame = false;//�������� �� ����� ����
    bool winGame = false;//�������� �� ������
    bool pause = false;//�������� �� �����
    bool checkVolumeAudio = false;//�������� �� ���/���� �����


    int[] finalStatistics = new int[4];//��������� ��� �������� ��������� ������, ������� ������� � ������, ���-�� ����

    // Start is called before the first frame update
    void Start()
    {
        WheatWrite();   //  
        PeasantWrite(); //
        WarWrite();     //����� ��������� ��������

        AttackWrite();//����� ���-�� ����������

        CheckButton();//��������� ������� �� �������� � ������ ���� ��� ����� ����-��
        
    }

    // Update is called once per frame
    void Update()
    {
       

        if (pause == false)
        {
            getWheatTime += Time.deltaTime;//������� ����� ��� ��������� ������
            WheatTimerImage.fillAmount = getWheatTime / 3;//���������� ������ ��������� ������

            if (getWheatTime >= 3)//��������� ������ �� 5 ������
            {
                wheat += 1 * peasant;//��������� �� ������� ������������ 1 �������
                finalStatistics[0] += 1 * peasant;//������� ������� ������ ������� �� ��� ����� ����
                CheckButton();//��������� ������� �� �������� ����� ����� ����� 
                CheckWinGame();//��������� ��������� ������� ������
                getWheatTime = 0;//�������� ����� �������� ������
                GameAudioSources[0].Play();//������������� ���� ����� ������
                WheatWrite();//����� �������
            }

            eatingTime += Time.deltaTime;//������� ���� �� ������ ����
            EatingTimerImage.fillAmount = eatingTime / 12;//���������� ������ ������ ����

            if (eatingTime >= 12)//��������� ������ �� 12 ������
            {
                totalPeople = peasant + war;//������� ����� ����� � �������
                wheat -= totalPeople;//�������� �� ������� ���-�� ��� ������� ����� ����
                eatingTime = 0;//�������� ����� ������ ����
                GameAudioSources[0].Play();//������������� ���� ������ ����
                WheatWrite();//����� ����
            }

            attackTime += Time.deltaTime;//������� ���� �� ���������
            AttackTimerImage.fillAmount = attackTime / attackTimeIncrease;//���������� ������ �� �����

            if (attackTime >= attackTimeIncrease)//��������� ������ �� 30 ������
            {
                GameAudioSources[2].Play();//������������� ���� �����
                if (war < attack)
                {
                    endGame = true;
                    pause = true;
                }
                else if(nullAttacker < 2)//��������� �������, ���� ������ 3 �� ����� ����� ��� ������
                {
                    nullAttacker++;//����������� ����������, ��� �� ����� ��������� ����� ������� � ������ � �������
                    attackTime = 0;//�������� ����� �� ��������� 
                    finalStatistics[3] += 1;
                }
                else
                {
                    finalStatistics[3] += 1;//������� ������� ��� ���� �� ��� ����� ����
                    attackTimeIncrease *= 1.1f;//����������� ����� �� ���� �����
                    war -= attack;//�������� �� ������ �������� ���-�� ������
                    attack += 1;//����������� ���-�� ����������
                    attackTime = 0;//�������� ����� �� ��������� 
                    WarWrite();//������ ���-�� ������
                    AttackWrite();
                }
            }


            if (hirePeasant == true)//���� ���� ������ ������ ��������� ����������� true
            {
                getPeasantTime += Time.deltaTime;//�������� ������� ��� ��������� ������ �����������
                PeasantTimerImage.fillAmount = getPeasantTime / 6;//���������� ������ ��������� ������ �����������

                if (getPeasantTime >= 6)//��������� ������ �� 6 ������
                {
                    peasant++;//��������� �����������
                    finalStatistics[1] += 1;//������� ������� �������� ���� ������ �� ��� ����� ����
                    PeasantButton.interactable = true;//������ ������ ��������
                    getPeasantTime = 0;//�������� ����� ��������� ������ �����������
                    hirePeasant = false;//������� �������� ������ �����������
                    PeasantTimerImage.fillAmount = 0;//�������� ����� ������� ������ �����������
                    PeasantWrite();//����� ����������� 
                    CheckButton();//��������� ������� �� �������� ����� ���������� ������� �����������
                    GameAudioSources[1].Play();//������������� ���� ��������� �����������
                }
            }

            if (hireWar == true)//���� ���� ������ ������ ��������� ����������� true
            {
                getWarTime += Time.deltaTime;//�������� ������� ��� ��������� ������ �����
                WarTimerImage.fillAmount = getWarTime / 8;//���������� ������ ��������� ������ �����

                if (getWarTime >= 8)//��������� ������ �� 8 ������
                {
                    war++;//��������� �����
                    finalStatistics[2] += 1;//������� ������� ������ ���� ������ �� ��� ����� ����
                    WarButton.interactable = true;//������ ������ ��������
                    getWarTime = 0;//�������� ����� ��������� ������ �����������
                    hireWar = false;//������� �������� ������ �����������
                    WarTimerImage.fillAmount = 0;//�������� ����� ������� ������ �����������
                    WarWrite();//����� ������
                    CheckWinGame();//��������� ��������� ������� ������
                    CheckButton();//��������� ������� �� �������� ����� ���������� ������� �����
                    GameAudioSources[1].Play();//������������� ���� ��������� �����
                }
            }
        }
        else if (winGame == true)
        {
            FinalStatistics();
            winGamePanel.SetActive(true);//�������� ������ ������
        }
        else if(endGame == true) 
        {

            FinalStatistics();
            endGamePanel.SetActive(true);//�������� ������ ���������
        }
    }


    /// <summary>
    /// �������� ��������� ������ �����������
    /// </summary>
    public void HirePeasant()
    {
        if(wheat >= 10)//���� ������� �������
        {
            hirePeasant = true;//����������� ��������� �� �������� �����������
            PeasantButton.interactable = false;//������ ������ ����������
            wheat -= 10;//�������� ������� ���������� �� �����������
            CheckButton();//��������� ������� �� �������� ����� ����� ������� �����������
            WheatWrite();//����� �������
        }
       
    }

    /// <summary>
    /// �������� ��������� ������ �����
    /// </summary>
    public void HireWar()
    {
        if (wheat >= 20)//���� ������� �������
        {
            hireWar = true;//����������� ��������� �� �������� �����
            WarButton.interactable = false;//������ ������ ����������
            wheat -= 20;//�������� ������� ���������� �� �����
            CheckButton();//��������� ������� �� �������� ����� ����� ������� �����
            WheatWrite();//����� �������

        }
    }
         
    /// <summary>
    /// ���������� �������� �� ���������
    /// </summary>
    public void RestartGame()
    {
        peasant = 5;
        war = 0;
        wheat = 20;

        getWheatTime = 0;
        getPeasantTime = 0;
        getWarTime = 0;
        eatingTime = 0;
        attackTime = 0;
        attackTimeIncrease = 30;

        hirePeasant = false;
        hireWar = false;
        endGame = false;
        pause = false;
        endGamePanel.SetActive(false);
        winGamePanel.SetActive(false);

        PeasantTimerImage.fillAmount = 0;
        WarTimerImage.fillAmount = 0;

        PeasantButton.interactable = true;
        WarButton.interactable = true;

        WheatWrite();
        PeasantWrite();
        WarWrite();
        AttackWrite();
    }

    /// <summary>
    /// ��������� ������� �� �������� �� ������� ����������� ��� ����� 
    /// </summary>
    void CheckButton()
    {
        if (wheat < 10 || hirePeasant == true) PeasantButton.interactable = false;//
        else PeasantButton.interactable = true;            //��������� ������ ��� ����� �����������

        if(wheat < 20 || hireWar == true) WarButton.interactable = false; //
        else WarButton.interactable = true;            //��������� ������ ��� ����� �����

    }

    /// <summary>
    /// ��������� ������� ��� ������ 
    /// </summary>
    void CheckWinGame()
    {
        if (war >= 10 && wheat >= 250)
        {
            winGame = true;
            pause = true;
        }
    }

    /// <summary>
    /// ������������� ��� ��������� ����
    /// </summary>
    public void PauseGame()
    {
        if (pause != true)
        {
            Time.timeScale = 0;//���� ����� �������� ������������� �����
            PausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;//���� ����� ��������� ��������� �����
            PausePanel.SetActive(false);
        }

        pause = !pause;//�������� �������� ����� ��� ������� ������
    }

    /// <summary>
    /// ��������� ��� ���������� �����
    /// </summary>
    public void VolumeAudio()
    {
        if (checkVolumeAudio != true)
        {
            AudioListener.volume = 0;//���� ���� �������� ������������� ��������� �� 0
            VolumeAudioButtonText.text = "��� ����";
        }
        else
        { 
            AudioListener.volume = 1;//���� ���� ������� ������������� ��������� �� 1
            VolumeAudioButtonText.text = "���� ����";
        }


        checkVolumeAudio = !checkVolumeAudio;//�������� �������� ���/���� �����
    }

    void WheatWrite() => WheatText.text = wheat.ToString();//����� �������
    void PeasantWrite() => PeasantText.text = peasant.ToString();//����� ��������
    void WarWrite() => WarText.text = war.ToString();//����� �����
    void AttackWrite() => QuantiAttackText.text = attack.ToString();//����� ���-�� ����������
    
    /// <summary>
    /// ���������� �������� ����� ����������
    /// </summary>
    void FinalStatistics()
    {
        FinalStatisticsText[0].text = finalStatistics[0].ToString();
        FinalStatisticsText[1].text = finalStatistics[1].ToString();
        FinalStatisticsText[2].text = finalStatistics[2].ToString();
        FinalStatisticsText[3].text = finalStatistics[3].ToString();
       
    }
}


