using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.UI;

public class VillageControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WheatText;//����� �������
    [SerializeField] TextMeshProUGUI PeasantText;//����� ��������
    [SerializeField] TextMeshProUGUI WarText;//����� ������

    [SerializeField] TextMeshProUGUI QuantiAttackText;//���-�� ����������


    [SerializeField] Image WheatTimerImage;//����������� ������� ������ ����� ������
    [SerializeField] Image EatingTimerImage;//����������� ������� ������ ����
    [SerializeField] Image AttackTimerImage;//����������� ������� ���������
    [SerializeField] Image PeasantTimerImage;//����������� ������� ��������� ������ �����������
    [SerializeField] Image WarTimerImage;//����������� ������� ��������� ������ �����


    [SerializeField] Button PeasantButton;//������ ������ �����������
    [SerializeField] Button WarButton;//������ ������ �����


    [SerializeField] GameObject endGamePanel;//������ ���������
    [SerializeField] GameObject winGamePanel;//������ ������

    int peasant = 5, 
        war = 0,
        wheat = 45; //���������� �������� �������
    int totalPeople;//����� �����
    int attack = 1;//���������� �����
    


    float getWheatTime, //����� ��� ��������� �������
          getPeasantTime,//����� ��� ��������� ������ �����������
          getWarTime,//����� ��� ��������� ������ �����
          eatingTime,//����� ������ ����
          attackTime,//����� ��������� 
          attackTimeIncrease = 3;//����� ��� ��������� 

    bool hirePeasant = false;//��������� ��� ������������ ������� ������ ��� �� ������ �����������
    bool hireWar = false;//��������� ��� ������������ ������� ������ ��� �� ������ �����
    bool endGame = false;//�������� �� ����� ����
    bool winGame = false;//�������� �� ������
    bool pause = false;
    

    // Start is called before the first frame update
    void Start()
    {
        WheatWrite();   //  
        PeasantWrite(); //
        WarWrite();     //����� ��������� ��������

        AttackWrite();//����� ���-�� ����������
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

                getWheatTime = 0;//�������� ����� �������� ������

                WheatWrite();//����� �������
            }

            eatingTime += Time.deltaTime;//������� ���� �� ������ ����
            EatingTimerImage.fillAmount = eatingTime / 12;//���������� ������ �����

            if (eatingTime >= 12)//��������� ������ �� 12 ������
            {
                totalPeople = peasant + war;//������� ����� ����� � �������
                wheat -= totalPeople;//�������� �� ������� ���-�� ��� ������� ����� ����
                eatingTime = 0;//�������� ����� ������ ����
                WheatWrite();//����� ����
            }

            attackTime += Time.deltaTime;//������� ���� �� ���������
            AttackTimerImage.fillAmount = attackTime / attackTimeIncrease;//���������� ������ �� �����

            if (attackTime >= attackTimeIncrease)//��������� ������ �� 30 ������
            {
                if (war < attack)
                {
                    endGame = true;
                    pause = true;
                }
                else
                {
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
                    PeasantButton.interactable = true;//������ ������ ��������
                    getPeasantTime = 0;//�������� ����� ��������� ������ �����������
                    hirePeasant = false;//������� �������� ������ �����������
                    PeasantTimerImage.fillAmount = 0;//�������� ����� ������� ������ �����������
                    PeasantWrite();//����� ����������� 
                }
            }

            if (hireWar == true)//���� ���� ������ ������ ��������� ����������� true
            {
                getWarTime += Time.deltaTime;//�������� ������� ��� ��������� ������ �����
                WarTimerImage.fillAmount = getWarTime / 8;//���������� ������ ��������� ������ �����

                if (getWarTime >= 8)//��������� ������ �� 8 ������
                {
                    war++;//��������� �����
                    WarButton.interactable = true;//������ ������ ��������
                    getWarTime = 0;//�������� ����� ��������� ������ �����������
                    hireWar = false;//������� �������� ������ �����������
                    WarTimerImage.fillAmount = 0;//�������� ����� ������� ������ �����������
                    WarWrite();//����� ����������� 
                }
            }

            if (war >= 5 && wheat >= 100)
            {
                winGame = true;
                pause = true;
            }

        }
        else if (winGame == true)
        {
            winGamePanel.SetActive(true);//�������� ������ ������
        }
        else if(endGame == true) 
        {
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

    void WheatWrite() => WheatText.text = wheat.ToString();//����� �������
    void PeasantWrite() => PeasantText.text = peasant.ToString();//����� ��������
    void WarWrite() => WarText.text = war.ToString();//����� �����
    void AttackWrite() => QuantiAttackText.text = attack.ToString();//����� ���-�� ����������

}
